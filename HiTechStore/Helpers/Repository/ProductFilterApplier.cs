using System.Linq.Expressions;

using HiTechStore.Helpers.URLFilterQuery;
using HiTechStore.Models;

using Microsoft.EntityFrameworkCore;

namespace HiTechStore.Helpers.Repository;

public static class ProductFilterApplier
{
    private static Expression CompareExpressionBuilder(string valuePropertyName, QueryOperator op, object value, ParameterExpression param)
    {
        var propValue = Expression.Property(
                       Expression.Property(param, nameof(ComponentPropertyValue.Value)), valuePropertyName);
        var left = Expression.Convert(
         propValue,
         value.GetType()
        );

        var right = Expression.Constant(value);
        var ifNotNull = op switch
        {
            QueryOperator.Equal => Expression.Equal(left, right),
            QueryOperator.GreaterThan => Expression.GreaterThan(left, right),
            QueryOperator.LessThan => Expression.LessThan(left, right),
            QueryOperator.GreaterThanOrEqual => Expression.GreaterThanOrEqual(left, right),
            QueryOperator.LessThanOrEqual => Expression.LessThanOrEqual(left, right),
            _ => throw new NotSupportedException()
        };

        var notNullLeft = Expression.Condition(
         Expression.NotEqual(propValue, Expression.Constant(null)),
         ifNotNull,
         Expression.Constant(false)
        );

        return notNullLeft;

    }
    private static Expression<Func<TItem, bool>> ProvideAppliedQueryFilterPropertyExpression<TItem>(PropertyFilter filter)
        where TItem : BaseItemPropertyValue
    {
        var (propertyName, op, (stringValue, numberValue, dateValue, booleanValue)) = filter;


        ParameterExpression param = Expression.Parameter(typeof(TItem), "prop");

        var propertyType = Expression.Property(
                                Expression.Property(param, nameof(BaseItemPropertyValue.Property)), nameof(Property.PropertyType));


        List<(PropertyType, string, object?)> typeValuePairs = new() {
                                                            (PropertyType.String,  nameof(PropertyValue.ValueString), stringValue),
                                                            (PropertyType.Number,  nameof(PropertyValue.ValueNumber),numberValue),
                                                            (PropertyType.DateTime, nameof(PropertyValue.ValueDateTime), dateValue),
                                                            (PropertyType.Boolean, nameof(PropertyValue.ValueBoolean), booleanValue)
                                                        };

        // Try to create a nested condition for each non-null value
        // PropertyType.Number == prop.PropertyType
        // ================ This is like ==================
        // prop.Property!.PropertyType == PropertyType.Number ?
        //     prop.Value!.ValueNumber == numberValue :
        // prop.Property.PropertyType == PropertyType.Boolean ?
        //     prop.Value!.ValueBoolean == booleanValue :
        // prop.Property.PropertyType == PropertyType.String ?
        //     prop.Value!.ValueString == stringValue :
        // false

        Expression? lastCondition = null;
        foreach (var (type, name, val) in typeValuePairs)
        {
            if (val is null)
            {
                continue;
            }

            if (type == PropertyType.String &&
                    (op != QueryOperator.In || op != QueryOperator.Equal || op != QueryOperator.In)
                )
            {
                continue;
            }

            lastCondition = Expression.Condition(
                                Expression.Equal(propertyType, Expression.Constant(type)),
                                CompareExpressionBuilder(name, op, val, param),
                                lastCondition is null ? Expression.Constant(false) : lastCondition
                            );
        }


        var ilikeExpr = Expression.Call(
            typeof(NpgsqlDbFunctionsExtensions),
            nameof(NpgsqlDbFunctionsExtensions.ILike),
            Type.EmptyTypes,
            Expression.Constant(EF.Functions),
            Expression.Property(
                Expression.Property(param, "Property"), "Name"
            ),
            Expression.Constant(propertyName)
        );

        Expression finalExpr = Expression.AndAlso(ilikeExpr, lastCondition ?? Expression.Constant(true));

        var lambda = Expression.Lambda<Func<TItem, bool>>(finalExpr, param);

        return lambda;
    }


    public static IQueryable<Product> Apply(IQueryable<Product> baseQuery,
        Dictionary<string, QueryFilterItem> productComponentFilters,
        CategoryFilters categoryFilters
        )
    {
        if (productComponentFilters.Any())
        {
            // There are two types of keys:
            // 1- Normal keys: single part keys which refer to a componentModel brandModel. 
            //    Like gpu=Nvidia which gpu refer to component type and Nvidia refer to brand-model.
            // 2- Multi-part keys: these keys are separated  by dot(.) character which parsed
            //    as <componentType>.<propertyName>. Like "gpu.VRam=8"
            var filterKeyTypeGroups = productComponentFilters
            .GroupBy(
                (filter) =>
                filter.Key.Split('.', 2).Length);

            var singlePartKeys = filterKeyTypeGroups.FirstOrDefault((g) => g.Key == 1)?.ToList() ?? [];
            var multiPartKeys = filterKeyTypeGroups.FirstOrDefault((g) => g.Key == 2)?.ToList() ?? [];

            IEnumerable<(string ComponentName, string PropertyName)> referedComponentProperties = multiPartKeys.Select(
                (mpk) =>
                {
                    var parts = mpk.Value.Name.Split('.', 2, StringSplitOptions.RemoveEmptyEntries |
                                                    StringSplitOptions.TrimEntries);

                    return (ComponentName: parts[0], PropertyName: parts[1]);
                }
            );


            var filters = multiPartKeys.Select(
                 (mpk) => FilterMapper.QueryFilterToComponentFilter(mpk.Value)
             );



            // Improvement: use expression abstract tree to handle which type must eliminates based on
            // if for that type the converted value is null
            // Improbement2: First combine all filters expression and then use it in queryBuilder
            foreach (var filter in filters)
            {
                IEnumerable<string> desiredComponentBrandModels = singlePartKeys.Where((f) => f.Key == filter.ComponentName)
                                                    .SelectMany(f => f.Value.GetValue<IEnumerable<string>>() ?? [])
                                                    .Select(mn => mn?.ToLower())
                                                    .Where(modelName => !string.IsNullOrWhiteSpace(modelName))!;

                baseQuery = baseQuery.Where(
                    (p) => p.ComponentModels.Any(
                        (cm) =>
                            (desiredComponentBrandModels.Count() > 0 ?
                            desiredComponentBrandModels.Contains(cm.BrandModel!.Brand!.NormalizedName)
                            : true) &&
                            EF.Functions.ILike(filter.ComponentName, cm.ComponentType!.Name!) &&
                            cm.Properties!.AsQueryable().Any(ProvideAppliedQueryFilterPropertyExpression<ComponentPropertyValue>(filter))
                    )
                );
            }

            if (categoryFilters.categoriesFilters is not null &&
                    categoryFilters.categoriesFilters.Any() &&
                    categoryFilters.Ids.Any())
            {
                foreach (var filter in categoryFilters.categoriesFilters)
                {
                    var propertyFilter = FilterMapper.QueryFilterToPropertyFilter(filter.Value);
                    if (propertyFilter is null)
                    { continue; }

                    baseQuery = baseQuery.Where(
                        (p) => categoryFilters.Ids.Contains(p.CategoryId) &&
                            p.Properties!.AsQueryable().Any(ProvideAppliedQueryFilterPropertyExpression<ProductPropertyValue>(propertyFilter))
                    );
                }
            }
        }
        return baseQuery;
    }

}


static class FilterMapper
{
    private static PropertyPossibleValues ProvidePropertyValues(QueryFilterItem filter)
    {
        // All types defined as Nullable because all PropertyValue.Value*** has defined as nullable,
        // and when we trying to using them in expression tree we not encounter different data type
        var stringValue = filter.Value;
        var numberValue = filter.GetValue<double>();
        bool? booleanValue = string.Equals(stringValue, "true", StringComparison.OrdinalIgnoreCase) ? true :
                           string.Equals(stringValue, "false", StringComparison.OrdinalIgnoreCase) ? false : null;
        var dateValue = filter.GetValue<DateTime>();

        return new PropertyPossibleValues(stringValue, numberValue, dateValue, booleanValue);
    }
    public static ComponentFilter QueryFilterToComponentFilter(QueryFilterItem filter)
    {
        var parts = filter.Name.Split('.', 2, StringSplitOptions.RemoveEmptyEntries |
                                        StringSplitOptions.TrimEntries);

        return new ComponentFilter(parts[0], parts[1], filter.Op, ProvidePropertyValues(filter));
    }

    public static PropertyFilter QueryFilterToPropertyFilter(QueryFilterItem filter)
    {
        return new PropertyFilter(filter.Name, filter.Op, ProvidePropertyValues(filter));
    }
}

record class PropertyFilter(string PropertyName, QueryOperator Operator, PropertyPossibleValues PossibleValues);
record class ComponentFilter(string ComponentName, string PropertyName, QueryOperator Operator, PropertyPossibleValues PossibleValues)
    : PropertyFilter(PropertyName, Operator, PossibleValues);
record struct PropertyPossibleValues(string? ValueString, double? ValueNumber, DateTime? ValueDateTime, bool? ValueBoolean);
public record class CategoryFilters(IEnumerable<int?> Ids, Dictionary<string, QueryFilterItem>? categoriesFilters);