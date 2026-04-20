
using System.Linq.Expressions;

using HiTechStore.Data.DTOs;
using HiTechStore.Data.DTOs.Brand;
using HiTechStore.Data.DTOs.Component;
using HiTechStore.Data.DTOs.Product;
using HiTechStore.Data.Queries;
using HiTechStore.Helpers.Repository;
using HiTechStore.Helpers.URLFilterQuery;
using HiTechStore.Helpers.URLFilterQuery.QueryAppliers;
using HiTechStore.Models;

namespace HiTechStore.Data.Repositories.Helpers;

public class ProductRepositoryHelper
{
    static public IQueryable<ProductDto> ToDtoProject(IQueryable<Product> baseQuery)
    {
        return baseQuery.Select(p => new ProductDto
        {
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
            Variations = p.Variations.Select(pv => new ProductVariationDto()
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
                Price = pv.Price
            }).ToList(),
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
        });
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
                Expression<Func<TProduct, object>> sorter = sortBy switch
                {
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