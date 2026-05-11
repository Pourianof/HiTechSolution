using AutoMapper;

using HiTechStore.Core.Repositories;
using HiTechStore.Data.DTOs.Product;
using HiTechStore.Data.Mapping;
using HiTechStore.Data.Queries;
using HiTechStore.Data.Repositories.Helpers;
using HiTechStore.Models;

using Microsoft.EntityFrameworkCore;

namespace HiTechStore.Data.Repositories
{
    public class ProductRepository : Repository<Product, ProductDto, ProductQuery>, IProductRepository
    {
        public ProductRepository(HiTechStoreDbContext context, IMapper mapper, IConditionComponentTreeToLambdaExpression conditionToExpressionMapper) : base(context, mapper)
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
        protected override IQueryable<ProductDto> HandleProject(IQueryable<Product> queryable)
        {
            return ProductRepositoryHelper.ToDtoProject(queryable);
        }

        protected override IQueryable<Product> GetAllQueryBuilder(IQueryable<Product> queryBuilder, ProductQuery? productQueryParams)
        {
            return ProductRepositoryHelper.ApplyQueryParams(
                queryBuilder,
                productQueryParams
            );
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

        public async Task<IEnumerable<ProductVariation>> GetAllVariations(IEnumerable<int> variationIds)
        {
            return await _context.Set<ProductVariation>().Where(
                (pv) => variationIds.Contains(pv.ProductVariationId)
            ).Include(pv => pv.Product).ToListAsync();
        }
    }
}

