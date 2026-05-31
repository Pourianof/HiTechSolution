using System.Linq.Expressions;
using System.Reflection;

using HiTechStore.Helpers.URLFilterQuery;
using HiTechStore.Helpers.URLFilterQuery.QueryAppliers;
using HiTechStore.Core.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;

namespace HiTechStore.Helpers.Repository;

public static class ProductFilterApplier
{
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
                    (op != QueryOperator.In || op != QueryOperator.Equal)
                )
            {
                continue;
            }

            Expression targetValueParam = Expression.Property(
                Expression.Property(
                    param, nameof(BaseItemPropertyValue.Value)
                ), name
            );

            if (targetValueParam is null)
            {
                continue;
            }

            var propertySpecifierLambdaExpression = Expression.Lambda(targetValueParam, param);

            var applierType = typeof(SinglePropertyQueryApplier<,>)
                .MakeGenericType(typeof(TItem), targetValueParam.Type);
            var constructor = applierType.GetConstructors().First();

            var applier = constructor.Invoke([propertySpecifierLambdaExpression]);

            MethodInfo applierMethod = applierType.GetMethod(nameof(SinglePropertyQueryApplier<string, string>.ApplyOperator))!;

            lastCondition = Expression.Condition(
                                Expression.Equal(propertyType, Expression.Constant(type)),
                                ((Expression<Func<TItem, bool>>)applierMethod!.Invoke(applier, [val, op])!).Body,
                                lastCondition is null ? Expression.Constant(false) : lastCondition
                            );
        }


        var ilikeExpr = Expression.Call(
            typeof(NpgsqlDbFunctionsExtensions),
            nameof(NpgsqlDbFunctionsExtensions.ILike),
            Type.EmptyTypes,
            Expression.Constant(EF.Functions),
            Expression.Property(
                Expression.Property(param, nameof(BaseItemPropertyValue.Property)), nameof(Property.Name)
            ),
            Expression.Constant(propertyName)
        );

        Expression finalExpr = Expression.AndAlso(ilikeExpr, lastCondition ?? Expression.Constant(true));

        var lambda = Expression.Lambda<Func<TItem, bool>>(finalExpr, param);

        return lambda;
    }


    public static IQueryable<TProduct> Apply<TProduct>(IQueryable<TProduct> baseQuery,
        Dictionary<string, QueryFilterItem> productComponentFilters,
        CategoryFilters categoryFilters
        ) where TProduct : Product
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

            var singlePartKeys = FilterMapper.MapQueryFiltersToFilterItems(
                                        filterKeyTypeGroups.FirstOrDefault((g) => g.Key == 1)?.ToDictionary()
                                );
            var multiPartKeys = FilterMapper.MapQueryFiltersToFilterItems(
                                    filterKeyTypeGroups.FirstOrDefault((g) => g.Key == 2)?.ToDictionary()
                                    );

            IEnumerable<(string ComponentName, string PropertyName)> referedComponentProperties = multiPartKeys.Select(
                (mpk) =>
                {
                    var parts = mpk.Name.Split('.', 2, StringSplitOptions.RemoveEmptyEntries |
                                                    StringSplitOptions.TrimEntries);

                    return (ComponentName: parts[0], PropertyName: parts[1]);
                }
            );

            if (multiPartKeys.Any())
            {
                var filters = multiPartKeys.Select(
                 FilterMapper.QueryFilterToComponentFilter
                );

                // Improvement: use expression abstract tree to handle which type must eliminates based on
                // if for that type the converted value is null
                // Improvement2: First combine all filters expression and then use it in queryBuilder
                // Improvement3: Current way to finding target products is: Products -> components -> properties -> properValues
                // in this way we must find the target products based on other tables, means that we loop over all products and
                // their properties to find which one is matched
                // But the suggested ways: categories -> components -> properties -> propertyvalues -> Products
                // in this way we loop over the more light weight and finite tables first and the find the target Product Ids
                foreach (var filter in filters)
                {
                    IEnumerable<string> desiredComponentBrandModels = singlePartKeys.Where((f) => f.Name == filter.ComponentName)
                                                        .SelectMany(f => f.GetValue<IEnumerable<string>>() ?? [])
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
            }
            else if (singlePartKeys.Any())
            {
                // Only brand Checks
                foreach (var targetComponent in singlePartKeys)
                {
                    var componentType = targetComponent.Name;
                    var brandNames = targetComponent.GetValue<IEnumerable<string>>()?.Select(v => v.ToLower());
                    if (brandNames != null && brandNames.Any())
                    {
                        baseQuery = baseQuery.Where(
                            (p) => p.ComponentModels.Any(
                                (cm) => cm.BrandModel != null &&
                                    EF.Functions.ILike(componentType, cm.ComponentType!.Name!) &&
                                    brandNames.Contains(cm.BrandModel.Brand!.NormalizedName)
                            )
                        );
                    }

                }
            }

            if (categoryFilters.categoriesFilters is not null &&
                    categoryFilters.categoriesFilters.Any() &&
                    categoryFilters.Ids.Any())
            {
                var categoryOperatorFilters = FilterMapper.MapQueryFiltersToFilterItems(categoryFilters.categoriesFilters);
                foreach (var filter in categoryOperatorFilters)
                {
                    var propertyFilter = FilterMapper.QueryFilterToPropertyFilter(filter);
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
    public static List<FilterItem> MapQueryFiltersToFilterItems(Dictionary<string, QueryFilterItem>? filters)
    {
        if (filters is null)
        {
            return new();
        }
        return filters.SelectMany(
                (item) => item.Value.AllFilters.Select((f) => new FilterItem(item.Value.FilterKey, f))
            ).ToList();
    }
    internal static PropertyPossibleValues ProvidePropertyValues(FilterItem filter)
    {
        var isInOp = filter.Operator == QueryOperator.In;
        // All types defined as Nullable because all PropertyValue.Value*** has defined as nullable,
        // and when we trying to using them in expression tree we not encounter different data type
        object? stringValue = isInOp ? filter.GetValue<IEnumerable<string>>() : filter.Value;
        object? numberValue = isInOp ? filter.GetValue<IEnumerable<double>>() : filter.GetValue<double>();
        bool? booleanValue = string.Equals(filter.Value, "true", StringComparison.OrdinalIgnoreCase) ? true :
                           string.Equals(filter.Value, "false", StringComparison.OrdinalIgnoreCase) ? false : null;
        object? dateValue = isInOp ? filter.GetValue<IEnumerable<DateTime>>() : filter.GetValue<DateTime>();

        return new PropertyPossibleValues(stringValue, numberValue, dateValue, booleanValue);
    }
    public static ComponentFilter QueryFilterToComponentFilter(FilterItem filter)
    {
        var parts = filter.Name.Split('.', 2, StringSplitOptions.RemoveEmptyEntries |
                                        StringSplitOptions.TrimEntries);

        return new ComponentFilter(parts[0], parts[1], filter.Operator, ProvidePropertyValues(filter));
    }

    public static PropertyFilter QueryFilterToPropertyFilter(FilterItem filter)
    {
        return new PropertyFilter(filter.Name, filter.Operator, ProvidePropertyValues(filter));
    }
}

record class PropertyFilter(string PropertyName, QueryOperator Operator, PropertyPossibleValues PossibleValues);
record class ComponentFilter(string ComponentName, string PropertyName, QueryOperator Operator, PropertyPossibleValues PossibleValues)
    : PropertyFilter(PropertyName, Operator, PossibleValues);
record struct PropertyPossibleValues(object? ValueString, object? ValueNumber, object? ValueDateTime, bool? ValueBoolean);
public record class CategoryFilters(IEnumerable<int?> Ids, Dictionary<string, QueryFilterItem>? categoriesFilters);

public class FilterItem(string name, OperatorValuePair pair)
{
    private OperatorValuePair _pair = pair;
    public string Name { get; } = name;
    public QueryOperator Operator { get; } = pair.Operator;
    public StringValues Value { get; } = pair.Value;

    public TValue? GetValue<TValue>()
    {
        return _pair.GetValue<TValue>();
    }
}