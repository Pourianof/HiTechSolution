using System.Linq.Expressions;

using HiTechStore.Core.Repositories;
using HiTechStore.Data.DTOs;
using HiTechStore.Data.DTOs.Brand;
using HiTechStore.Data.DTOs.Component;
using HiTechStore.Data.DTOs.Product;
using HiTechStore.Data.Mapping;
using HiTechStore.Data.Queries;
using HiTechStore.Data.Repositories.Helpers;
using HiTechStore.Helpers.Expression;
using HiTechStore.Helpers.URLFilterQuery;
using HiTechStore.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;

namespace HiTechStore.Data.Repositories;

public class DiscountedProductsRepository(
    IConditionComponentTreeToLambdaExpression _conditionToExpressionMapper,
    HiTechStoreDbContext dbContext
) : IDiscountedProductsRepository
{
    private DbSet<Product> _dbSet = dbContext.Products;
    private IQueryable<ProductDto> ProjectWithDiscountData(IEnumerable<RuleCondition> ruleConditions, ProductQuery? productQuery)
    {
        Expression<Func<ProductWithMinMaxPrice, ProductWithMinMaxDiscount>> productDiscount =
                        (ProductWithMinMaxPrice p) => new ProductWithMinMaxDiscount
                        {
                            AuthorId = p.AuthorId,
                            BrandModel = p.BrandModel,
                            Category = p.Category,
                            ComponentModels = p.ComponentModels,
                            CreatedAt = p.CreatedAt,
                            Description = p.Description,
                            IsDeleled = p.IsDeleled,
                            ProductId = p.ProductId,
                            Properties = p.Properties,
                            Title = p.Title,
                            Variations = p.Variations,
                            Scores = p.Scores,
                        };

        var param = productDiscount.Parameters[0];

        Expression minDiscountSumExpression = Expression.Constant(0d);
        Expression maxDiscountSumExpression = Expression.Constant(0d);

        Expression isDiscountApply = Expression.Constant(false);

        foreach (var rCondition in ruleConditions)
        {

            if (rCondition.Action!.Value == 0)
            {
                continue;
            }

            var conditionBody = ExpressionParameterReplacer.ReplaceParameter(rCondition.Expression!, param).Body;

            isDiscountApply = Expression.OrElse(
                isDiscountApply,
                conditionBody
            );

            var value = Expression.Constant((double)rCondition.Action!.Value);
            var minPriceParam = Expression.Property(param, nameof(ProductWithMinMaxPrice.MinPrice));
            var maxPriceParam = Expression.Property(param, nameof(ProductWithMinMaxPrice.MaxPrice));

            var isPercentageBaseDiscount = rCondition.Action!.Type == DiscountActionType.Percent;

            // 1- maxPrice for min discount because in fixed-discounts division of 
            //    price/discountInDollar create smaller number
            // 2- if the rule is appliable and the condition established, then we 
            //    apply the discount to product
            Expression minDiscount = Expression.Condition(
                    conditionBody,
                    isPercentageBaseDiscount ?
                    value :
                    Expression.Multiply(
                        Expression.Divide(value, maxPriceParam),
                        Expression.Constant(100.0)),
                    Expression.Constant(0.0)
                );

            Expression maxDiscount = Expression.Condition(
                    conditionBody,
                    isPercentageBaseDiscount ?
                    value :
                    Expression.Multiply(
                        Expression.Divide(value, minPriceParam),
                        Expression.Constant(100.0)),
                    Expression.Constant(0.0)
                );

            minDiscountSumExpression = Expression.Add(
                minDiscountSumExpression, minDiscount
            );

            maxDiscountSumExpression = Expression.Add(
                maxDiscountSumExpression, maxDiscount
            );
        }

        // setting the MinDiscount/MaxDiscount initializing Expression
        var productDiscountToProductDtoProjection = productDiscount.ModifyProjection(
            new()
            {
                [nameof(ProductWithMinMaxDiscount.MinDiscount)] = minDiscountSumExpression,
                [nameof(ProductWithMinMaxDiscount.MaxDiscount)] = maxDiscountSumExpression
            }
        );

        // 1- query for calculate products min/max variation's price 
        // 1/1- along with filtering the discounted products
        // 2- then calculate the min/max total discount based-on percentage
        var baseQuery = _dbSet.Select(p => new ProductWithMinMaxPrice
        {
            MinPrice = p.Variations.Min(pv => pv.Price),
            MaxPrice = p.Variations.Max(pv => pv.Price),
            AuthorId = p.AuthorId,
            BrandModel = p.BrandModel,
            Category = p.Category,
            ComponentModels = p.ComponentModels,
            CreatedAt = p.CreatedAt,
            Description = p.Description,
            IsDeleled = p.IsDeleled,
            ProductId = p.ProductId,
            Properties = p.Properties,
            Title = p.Title,
            Variations = p.Variations,
            Scores = p.Scores,
        })
        .Where(
            Expression.Lambda<Func<ProductWithMinMaxPrice, bool>>(
                isDiscountApply,
                [param]
            )
        ).Select(productDiscountToProductDtoProjection);


        var isDes = productQuery?.SortDir?.GetValue<string>(QueryOperator.Equal) == "des";

        // default sort by discount-amount
        productQuery ??= new();
        productQuery.SortBy ??= new QueryFilterItem("sortBy")
            .AddOperatorValuePair(QueryOperator.Equal, new StringValues("discount"));
        productQuery.SortDir ??= new QueryFilterItem("sortDir")
        .AddOperatorValuePair(QueryOperator.Equal, new StringValues("des"));

        // product-specific query applier
        var discountBaseQuery = ProductRepositoryHelper.ApplyQueryParams(
            baseQuery,
            productQuery,
            new()
            {
                ["discount"] = isDes ? (p) => p.MaxDiscount : (p) => p.MinDiscount
            }
        );

        // general query applier
        discountBaseQuery = RepositoryHelper.ApplyGenericQuery(
            discountBaseQuery,
            productQuery
        );


        // map to ProductDto
        return discountBaseQuery
        .Select(p => new ProductDto
        {
            ProductId = p.ProductId,
            Title = p.Title,
            AverageScore = p.Scores.Any()
                                ? p.Scores.Average(s => (double)s.Score)
                                : 0.0,
            ScoreCounts = p.Scores.Count(),
            AuthorId = p.AuthorId,
            Variations = p.Variations.Select(
                    pv => new ProductVariationDto()
                    {
                        ProductVariationId = pv.ProductVariationId,
                        Color = pv.Color,
                        Inventory = pv.Inventory,
                        Media = pv.Media.Select(m => new ProductMediaDto()
                        {
                            IsMain = m.IsMain,
                            ProductMediaId = m.ProductMediaId,
                            Url = m.FilePath,
                            Type = m.Type == MediaType.Image ? "Image" : "Video"
                        }).ToList(),
                        Price = pv.Price,
                    }
                ).ToList(),
            BrandModel = new BrandModelDto
            {
                BrandName = p.BrandModel!.Brand!.Name,
                ModelName = p.BrandModel.Name,
                Descriotion = p.BrandModel.Description,
                ModelId = p.BrandModel.BrandModelId
            },
            Components = p.Category!.Components!.Select(
                    (c) => new ProductComponentDto()
                    {
                        Name = c.Component!.Name,
                        ComponentTypeId = c.ComponentTypeId,
                        Description = c.Component!.Description,
                        Models = p.ComponentModels.Where(m => m.ComponentTypeId == c.ComponentTypeId).Select(
                            (m) => new ComponentModelDto()
                            {
                                BrandModel = m.BrandModel != null ? new BrandModelDto()
                                {
                                    BrandName = m.BrandModel.Brand!.Name,
                                    Descriotion = m.BrandModel.Description,
                                    ModelId = m.BrandModel.BrandModelId,
                                    ModelName = m.BrandModel.Name
                                } : null,
                                ComponentModelId = m.ComponentModelId,
                                ComponentTypeId = c.ComponentTypeId,
                                Description = m.Description,
                                Properties = m.Properties!.Select(
                                    (prop) => new PropertyValueDto()
                                    {
                                        Name = prop.Property!.Name,
                                        PropertyId = prop.PropertyId,
                                        Value = prop.Value!.ValueNumber != null ? (object?)prop.Value!.ValueNumber :
                                                prop.Value!.ValueDateTime != null ? (object?)prop.Value!.ValueDateTime :
                                                prop.Value!.ValueBoolean != null ? (object?)prop.Value!.ValueBoolean :
                                                prop.Value!.ValueString,
                                        ValueType = prop.Property.PropertyType
                                    }
                                )
                            }
                        )
                    }
                ).ToList(),
            CategoryId = p.CategoryId,
            Description = p.Description,
            Properties = p.Properties.Select(
                    (prop) => new PropertyValueDto()
                    {
                        Name = prop.Property!.Name,
                        PropertyId = prop.ProductId,
                        Value = prop.Value!.ValueNumber != null ? (object?)prop.Value!.ValueNumber :
                                prop.Value!.ValueDateTime != null ? (object?)prop.Value!.ValueDateTime :
                                prop.Value!.ValueBoolean != null ? (object?)prop.Value!.ValueBoolean :
                                prop.Value!.ValueString,
                        ValueType = prop.Property.PropertyType
                    }
                ).ToList()
        }
        );
    }

    public async Task<IEnumerable<ProductDto>> GetDiscountedProducts(IEnumerable<DiscountRule> rules, ProductQuery? productQuery = default)
    {
        if (!rules.Any())
        {
            return [];
        }

        var ruleConditions = rules.Select(
            r => new RuleCondition
            {
                Action = r.DiscountAction,
                Expression = _conditionToExpressionMapper.Map<Product>(r.ProductConditionTree!)
            }
        );

        return await ProjectWithDiscountData(ruleConditions, productQuery).ToListAsync();
    }

    public async Task<IEnumerable<ProductDto>> GetProductsByCondition(ConditionComponent conditionTree)
    {
        var conditionLambda = _conditionToExpressionMapper.Map<Product>(conditionTree);

        return await ProductRepositoryHelper.ToDtoProject(
            _dbSet.Where(
                conditionLambda
            )
        ).ToListAsync();
    }
}


class RuleCondition
{
    public DiscountAction? Action { get; set; }
    public Expression<Func<Product, bool>>? Expression { get; set; }
}

class ProductWithMinMaxPrice : Product
{
    public double MinPrice { get; set; }
    public double MaxPrice { get; set; }

}

class ProductWithMinMaxDiscount : Product
{
    public double MinDiscount { get; set; }
    public double MaxDiscount { get; set; }
}