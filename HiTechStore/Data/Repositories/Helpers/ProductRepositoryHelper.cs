
using System.Linq.Expressions;

using HiTechStore.Data.DTOs;
using HiTechStore.Data.DTOs.Brand;
using HiTechStore.Data.DTOs.Component;
using HiTechStore.Data.DTOs.Product;
using HiTechStore.Data.Queries;
using HiTechStore.Helpers.Expressions;
using HiTechStore.Helpers.Repository;
using HiTechStore.Helpers.URLFilterQuery;
using HiTechStore.Helpers.URLFilterQuery.QueryAppliers;
using HiTechStore.Models;

using Microsoft.EntityFrameworkCore;

namespace HiTechStore.Data.Repositories.Helpers;

public class ProductRepositoryHelper
{
    static public IQueryable<ProductDto> ToDtoProject(IQueryable<Product> baseQuery, IEnumerable<string>? inclusions = default)
    {
        inclusions ??= [];
        Expression<Func<Product, ProductDto>> projector = p => new ProductDto
        {
            Inclusions = inclusions,
            ProductId = p.ProductId,
            Title = p.Title,
            AverageScore = p.Scores.Any()
                                ? p.Scores.Average(s => (double)s.Score)
                                : 0.0,
            ScoreCounts = p.Scores.Count(),
            AuthorId = p.AuthorId,
            BrandModel = new BrandModelDto
            {
                BrandName = p.BrandModel!.Brand!.Name,
                ModelName = p.BrandModel.Name,
                Description = p.BrandModel.Description,
                ModelId = p.BrandModel.BrandModelId
            },
            CategoryId = p.CategoryId,
            Description = p.Description,
        };

        if (inclusions is not null && inclusions.Count() > 0)
        {
            foreach (var includedPropertyName in inclusions)
            {
                if (string.Equals(includedPropertyName, nameof(ProductDto.Components), StringComparison.OrdinalIgnoreCase))
                {
                    Expression<Func<Product, List<ProductComponentDto>>> expression = (Product p) => p.Category!.Components!.Select(
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
                                                Description = m.BrandModel.Description,
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
                            ).ToList();

                    projector = projector.ModifyProjection(
                        new()
                        {
                            [nameof(ProductDto.Components)] = ExpressionParameterReplacer.ReplaceParameter(expression, projector.Parameters.First()).Body
                        }
                    );
                }
                else if (string.Equals(includedPropertyName, nameof(ProductDto.Variations), StringComparison.OrdinalIgnoreCase))
                {
                    Expression<Func<Product, List<ProductVariationDto>>> expression = (Product p) => p.Variations.Select(pv => new ProductVariationDto()
                    {
                        ProductVariationId = pv.ProductVariationId,
                        Color = pv.Color,
                        Inventory = pv.Inventory,
                        Media = pv.Media.Select(m => new ProductMediaDto()
                        {
                            IsMain = m.IsMain,
                            ProductMediaId = m.ProductMediaId,
                            Url = m.FilePath,
                            Type = m.Type == MediaType.Image ? "Image" : "Video",
                            ThumbnailUrl = m.ThumnailPath
                        }).ToList(),
                        Price = pv.Price
                    }).ToList();

                    projector = projector.ModifyProjection(
                        new()
                        {
                            [nameof(ProductDto.Variations)] = ExpressionParameterReplacer.ReplaceParameter(expression, projector.Parameters.First()).Body
                        }
                    );
                }
                else if (string.Equals(includedPropertyName, nameof(ProductDto.Properties), StringComparison.OrdinalIgnoreCase))
                {
                    Expression<Func<Product, List<PropertyValueDto>>> expression = (Product p) => p.Properties.Select(
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
                        ).ToList();

                    projector = projector.ModifyProjection(
                        new()
                        {
                            [nameof(ProductDto.Properties)] = ExpressionParameterReplacer.ReplaceParameter(expression, projector.Parameters.First()).Body
                        }
                    );
                }
            }
        }


        return baseQuery.Select(projector);
    }

    public static IQueryable<TProduct> ApplyQueryParams<TProduct>(
        IQueryable<TProduct> queryBuilder,
        ProductQuery? productQueryParams = default,
        Dictionary<string, Expression<Func<TProduct, object>>>? sortyByDelegate = default
    ) where TProduct : Product
    {
        if (productQueryParams is not null)
        {
            var categoryId = productQueryParams.Category?.GetValues<int>(QueryOperator.Equal)?.FirstOrDefault();
            if (categoryId is not null)
            {
                queryBuilder = queryBuilder.Where((p) => categoryId == p.CategoryId);
            }

            var priceFilters = productQueryParams.Price?.GetFilters(
                     QueryOperator.GreaterThan |
                     QueryOperator.GreaterThanOrEqual |
                     QueryOperator.LessThan |
                     QueryOperator.LessThanOrEqual
             );
            if (priceFilters is not null && priceFilters.Count() > 0)
            {
                queryBuilder = queryBuilder.ApplyFiltersTo<TProduct, double>(
                        priceFilters,
                        new CollectionQueryApplier<TProduct, double, ProductVariation>(
                            (TProduct product) => product.Variations,
                            pv => pv.Price
                        )
                );
            }

            var brandFilters = productQueryParams.Brand?.GetFilters(
                     QueryOperator.In |
                     QueryOperator.Equal
             );
            if (brandFilters is not null && brandFilters.Count() > 0)
            {
                queryBuilder = queryBuilder.ApplyFiltersTo<TProduct, string>(
                        brandFilters, new SinglePropertyQueryApplier<TProduct, string>(
                            (TProduct product) => product.BrandModel!.Brand!.Name!
                        )
                );
            }

            queryBuilder = ProductFilterApplier.Apply(queryBuilder, productQueryParams.FilterMaps,
                            new CategoryFilters([categoryId], productQueryParams.CategoryProperties));

            var sortBy = productQueryParams.SortBy?.GetValue<string>(QueryOperator.Equal);
            if (sortBy is not null)
            {
                Func<Expression<Func<TProduct, object>>> bestSellerFilter = () =>
                {
                    var bestSellingRange = productQueryParams?.BestSeller?.GetValue<string>(QueryOperator.Equal) ?? "month"; // "week", "month", "year"

                    var until = DateTime.UtcNow;
                    var from = bestSellingRange.ToLower() switch
                    {
                        "week" => until.AddDays(-7),
                        "year" => until.AddYears(-1),
                        _ => until.AddMonths(-1),
                    };

                    return (TProduct p) =>

                                // total sales count in range
                                p.Variations.Sum(pv => pv.Orders!.Count(o => o.Order!.PaymentState == OrderPaymentState.Paid && o.Order!.CreatedAt >= from && o.Order.CreatedAt <= until))
                                // plus score
                                + ((p.AverageScore ?? 0) / 5 * p.ScoreCounts);
                };

                Expression<Func<TProduct, object>> sorter = sortBy switch
                {
                    "best_sellers" => bestSellerFilter(),
                    "created_at" => (TProduct p) => p.CreatedAt,
                    "price" => productQueryParams.SortDir?.GetValue<string>(QueryOperator.Equal) == "des" ?
                            (TProduct p) =>
                                p.Variations.Max(pv => pv.Price) :
                            (TProduct p) =>
                                p.Variations.Min(pv => pv.Price),
                    _ => sortyByDelegate?.GetValueOrDefault(sortBy) ?? ((TProduct p) => p.CreatedAt)
                };
                queryBuilder = queryBuilder.OrderBy(sorter);
            }
            else
            {
                queryBuilder = queryBuilder.OrderBy((TProduct p) => p.CreatedAt).OrderDescending();
            }

        }

        return queryBuilder;
    }
}