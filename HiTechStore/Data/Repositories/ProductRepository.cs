using System.Diagnostics;
using System.Linq.Expressions;

using AutoMapper;

using HiTechStore.Core.Repositories;
using HiTechStore.Data.DTOs;
using HiTechStore.Data.DTOs.Brand;
using HiTechStore.Data.DTOs.Component;
using HiTechStore.Data.DTOs.Product;
using HiTechStore.Data.Queries;
using HiTechStore.Helpers.Repository;
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
        /**
            Override the Project method for handling the projection manually because:
            we wanted to display products components in this way:
            {
                components: {
                    <... component fields ...>
                    componentModels: [
                        <..available models of this component-type for this product...>
                    ]
                }
            }
            for this reason we could not generate a Automapper configuration which lead
            to a appropriate sql which give us the component-models which associate to
            target product
        */
        protected override IQueryable<ProductDto> Project(IQueryable<Product> queryable)
        {
            return queryable.Select(p => new ProductDto
            {
                ProductId = p.ProductId,
                Title = p.Title,
                AverageScore = p.Scores.Any()
                                ? p.Scores.Average(s => (double)s.Score)
                                : 0.0,
                ScoreCounts = p.Scores.Count(),
                AuthorId = p.AuthorId,
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
                Price = p.Price,
                CategoryId = p.CategoryId,
                Description = p.Description,
                Media = p.Media.Select(m => new ProductMediaDto()
                {
                    IsMain = m.IsMain,
                    ProductMediaId = m.ProductMediaId,
                    Url = m.FilePath,
                    Type = m.Type == MediaType.Image ? "Image" : "Video"
                }).ToList(),
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

        protected override IQueryable<Product> GetAllQueryBuilder(IQueryable<Product> queryBuilder, ProductQuery? productQueryParams)
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
                    queryBuilder = ProductFilterApplier.ApplyFiltersTo(
                            queryBuilder, (Product product) => product.Price, priceFilters
                    );
                }

                var brandFilters = productQueryParams.Brand?.GetFilters(
                         QueryOperator.In |
                         QueryOperator.Equal
                 );
                if (brandFilters is not null && brandFilters.Count() > 0)
                {
                    queryBuilder = ProductFilterApplier.ApplyFiltersTo(
                            queryBuilder, (Product product) => product.BrandModel!.Brand!.Name, brandFilters
                    );
                }

                queryBuilder = ProductFilterApplier.Apply(queryBuilder, productQueryParams.FilterMaps,
                                new CategoryFilters([categoryId], productQueryParams.CategoryProperties));

                var sortBy = productQueryParams.SortBy?.GetValue<string>(QueryOperator.Equal);
                if (sortBy is not null)
                {

                    Expression<Func<Product, object>> sorter = sortBy switch
                    {
                        "created_at" => (Product p) => p.CreatedAt,
                        "price" => (Product p) => p.Price,
                        _ => (Product p) => p.CreatedAt
                    };
                    queryBuilder = queryBuilder.OrderBy(sorter);
                }
                else
                {
                    queryBuilder = queryBuilder.OrderBy((Product p) => p.CreatedAt).OrderDescending();
                }

            }
            return queryBuilder;
        }

        protected override IQueryable<Product> GetByIdAsyncQueryBuilder(IQueryable<Product> queryBuilder)
        {
            return queryBuilder;
        }

        public async Task<ProductDto?> GetByIdAsync(int id, string? userId)
        {
            var query = _dbSet.Where(p => p.ProductId == id);
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

