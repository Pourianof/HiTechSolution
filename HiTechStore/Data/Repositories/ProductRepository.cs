using System.IO.Compression;
using System.Linq.Expressions;

using AutoMapper;

using HiTechStore.Core.Repositories;
using HiTechStore.Data.DTOs.Product;
using HiTechStore.Data.Queries;
using HiTechStore.Helpers.URLFilterQuery;
using HiTechStore.Models;

using Microsoft.EntityFrameworkCore;

namespace HiTechStore.Data.Repositories
{
    public class ProductRepository : Repository<Product, ProductDto, ProductQuery>, IProductRepository
    {
        public ProductRepository(HiTechStoreDbContext context, IMapper mapper) : base(context, mapper)
        {

        }

        private IQueryable<Product> BaseGettingQuery(IQueryable<Product> queryBuilder)
        {
            return queryBuilder.Select(p => new Product
            {
                ProductId = p.ProductId,
                Title = p.Title,
                AverageScore = p.Scores.Any()
                                 ? p.Scores.Average(s => (double?)s.Score)
                                 : 0.0,
                ScoreCounts = p.Scores.Count(),
                AuthorId = p.AuthorId,
                ComponentModels = p.ComponentModels,
                Author = p.Author,
                BrandModel = p.BrandModel,
                Category = p.Category,
                CategoryId = p.CategoryId,
                CreatedAt = p.CreatedAt,
                MyScore = p.MyScore,
                Description = p.Description,
                Price = p.Price,
                Media = p.Media,
                Properties = p.Properties
            });
        }

        protected override IQueryable<Product> GetAllQueryBuilder(IQueryable<Product> queryBuilder, ProductQuery? productQueryParams)
        {

            if (productQueryParams is not null)
            {
                if (productQueryParams.Category is not null)
                {
                    queryBuilder = queryBuilder.Where((p) => productQueryParams.Category.Value == p.CategoryId);
                }

                if (productQueryParams.FilterMaps.Count() > 0)
                {
                    // There are two types of keys:
                    // 1- Normal keys: single part keys which refer to a componentModel brandModel. 
                    //    Like gpu=Nvidia which gpu refer to component type and Nvidia refer to brand-model.
                    // 2- Multi-part keys: these keys are separated  by dot(.) character which parsed
                    //    as <componentType>.<propertyName>. Like "gpu.VRam=8"
                    var filterKeyTypeGroups = productQueryParams.FilterMaps
                    .GroupBy(
                        (filter) =>
                        filter.Key.Split('.', 2).Length);

                    var singlePartKeys = filterKeyTypeGroups.FirstOrDefault((g) => g.Key == 1)?.ToList() ?? [];
                    var multiPartKeys = filterKeyTypeGroups.FirstOrDefault((g) => g.Key == 2)?.ToList() ?? [];

                    IEnumerable<(string ComponentName, string PropertyName)> referedComponentProperties = multiPartKeys.Select(
                        (mpk) =>
                        {
                            var parts = mpk.Key.Split('.', 2, StringSplitOptions.RemoveEmptyEntries |
                                                            StringSplitOptions.TrimEntries);

                            return (ComponentName: parts[0], PropertyName: parts[1]);
                        }
                    );


                    var filters = multiPartKeys.Select(
                         (mpk) =>
                         {
                             var (key, filter) = mpk;
                             var parts = key.Split('.', 2, StringSplitOptions.RemoveEmptyEntries |
                                                             StringSplitOptions.TrimEntries);

                             // All types defined as Nullable because all PropertyValue.Value*** has defined as nullable,
                             // and when we trying to using them in expression tree we not encounter different data type
                             var stringValue = filter.Value;
                             var numberValue = filter.GetValue<double>();
                             bool? booleanValue = string.Equals(stringValue, "true", StringComparison.OrdinalIgnoreCase) ? true :
                                                string.Equals(stringValue, "false", StringComparison.OrdinalIgnoreCase) ? false : null;
                             var dateValue = filter.GetValue<DateTime>();

                             return new ComponentFilter(parts[0], parts[1], mpk.Value.Op, new PropertyPossibleValues(stringValue, numberValue, dateValue, booleanValue));
                         }
                     );

                    var compareExpressionBuilder = (string valuePropertyName, QueryOperator op, object value, ParameterExpression param) =>
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

                       };

                    // Improvement: use expression abstract tree to handle which type must eliminates based on
                    // if for that type the converted value is null

                    foreach (var filter in filters)
                    {
                        var (componentTypeName, propertyName, op, (stringValue, numberValue, dateValue, booleanValue)) = filter;

                        IEnumerable<string> desiredComponentBrandModels = singlePartKeys.Where((filter) => filter.Key == componentTypeName)
                                                            .Select(f => f.Value.GetValue<string>()?.ToLower())
                                                            .Where(modelName => !string.IsNullOrWhiteSpace(modelName))!;


                        ParameterExpression param = Expression.Parameter(typeof(ComponentPropertyValue), "prop");

                        var propertyType = Expression.Property(
                                                Expression.Property(param, nameof(ComponentPropertyValue.Property)), nameof(Property.PropertyType));


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
                                                compareExpressionBuilder(name, op, val, param),
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

                        var lambda = Expression.Lambda<Func<ComponentPropertyValue, bool>>(finalExpr, param);

                        queryBuilder = queryBuilder.Where(
                            (p) => p.ComponentModels.Any(
                                (cm) =>
                                    desiredComponentBrandModels.Count() > 0 ?
                                    desiredComponentBrandModels.Contains(cm.BrandModel!.Brand!.NormalizedName)
                                    : true &&
                                    EF.Functions.ILike(componentTypeName, cm.ComponentType!.Name!) &&
                                    cm.Properties!.AsQueryable().Any(lambda)
                            )
                        );
                    }
                }


            }
            return BaseGettingQuery(queryBuilder);
        }

        protected override IQueryable<Product> GetByIdAsyncQueryBuilder(IQueryable<Product> queryBuilder)
        {
            return BaseGettingQuery(queryBuilder);
        }

        public async Task<ProductDto?> GetByIdAsync(int id, string? userId)
        {
            var query = BaseGettingQuery(_dbSet).Where(p => p.ProductId == id);
            if (userId is string)
            {
                query.Where(p => p.AuthorId == userId);
            }
            return await Project(query).FirstOrDefaultAsync();

        }

        public override async Task Delete(int id)
        {
            await _dbSet.Where((p) => p.ProductId == id)
                  .ExecuteUpdateAsync(
                      (setter) =>
                          setter.SetProperty((prod) => prod.IsDeleled, true)
                  );

        }

        public override Task Delete(Product product)
        {
            product.IsDeleled = true;
            _context.Entry(product).Property((p) => p.IsDeleled).IsModified = true;
            return Task.CompletedTask;
        }
    }
}

record struct ComponentFilter(string ComponentName, string PropertyName, QueryOperator Operator, PropertyPossibleValues PossibleValues);
record struct PropertyPossibleValues(string? ValueString, double? ValueNumber, DateTime? ValueDateTime, bool? ValueBoolean);