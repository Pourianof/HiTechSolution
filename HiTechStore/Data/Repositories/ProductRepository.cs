using System.IO.Compression;
using System.Linq.Expressions;

using AutoMapper;

using HiTechStore.Core.Repositories;
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

                queryBuilder = ProductFilterApplier.Apply(queryBuilder, productQueryParams.FilterMaps);

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