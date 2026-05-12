using AutoMapper;

using HiTechStore.Core.Repositories;
using HiTechStore.Data.DTOs;
using HiTechStore.Data.DTOs.Product;
using HiTechStore.Data.Mapping;
using HiTechStore.Data.Queries;
using HiTechStore.Data.Repositories.Helpers;
using HiTechStore.Helpers.Types;
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

        public async Task<IEnumerable<ProductDto>> GetSimilarProductsOf(int productId)
        {
            // this similarity not contained Components and their properties and brands matching
            // just based on these criterias: 
            // * category(5) 
            // * price(2)
            // * brand(1) 
            // * brandModel(1)
            // * category property(0.5)
            var query = _dbSet.FindById(productId)
               .Select(
                   p => new
                   {
                       p.CategoryId,
                       p.BrandModel!.BrandModelId,
                       p.BrandModel!.BrandId,
                       AveragePrice = p.Variations.Average(pv => pv.Price),
                       Properties = p.Properties.Where(prop => prop.Property!.PropertyType == PropertyType.Number).Select(
                        prop => new
                        {
                            prop.Value!.ValueNumber!.Value,
                            prop.PropertyId
                        }
                       )
                   }
               ).Join(
                _dbSet,
                targetProduct => 1,
                similars => 1,
                (tp, s) => new { TargetProduct = tp, Similar = s }
               ).OrderByDescending(
                p => (p.Similar.CategoryId == p.TargetProduct!.CategoryId ? 5 : 0) + // category matching: 5 point
                     2 * (1 - (Math.Max(Math.Abs(p.Similar.Variations.Average(pv => pv.Price) - p.TargetProduct!.AveragePrice), 0) / p.TargetProduct.AveragePrice)) + // 2 - (1- Math.Max(0, Math.abs(diff)) / price) => a rank between 0 and 2 for price, max for removing negative result and abs for calculating distance regardless of direction
                     (p.Similar.BrandModel!.BrandId == p.TargetProduct.BrandId ? 1 : 0) + // brand matching : 1 point
                     (p.Similar.BrandModel!.BrandModelId == p.TargetProduct.BrandModelId ? 1 : 0) + // brand model matching : 1 point
                     p.Similar.Properties.Where(
                        similarProductProperty => similarProductProperty.Property!.PropertyType == PropertyType.Number &&
                            p.TargetProduct.Properties.Any(
                                targetProductProperty => similarProductProperty.PropertyId == targetProductProperty.PropertyId
                            )
                    )
                    .Sum(
                        propValue => 0.5 *
                            (1 - (Math.Max(Math.Abs(propValue.Value!.ValueNumber!.Value - p.TargetProduct.Properties.First(prop => prop.PropertyId == propValue.PropertyId).Value), 0) / p.TargetProduct.Properties.First(prop => prop.PropertyId == propValue.PropertyId).Value)) // 0.5 point for each property matching
                    )
             ).Take(10).Select(p => p.Similar);


            return await Project(query)
            .ToListAsync();
        }

        public Task<PagedResultDto<ProductDto>> GetPoductsOfUser(string userId, ProductQuery? productQuery)
        {
            return GetPagedResult<ProductDto>(
                _dbSet.Where(
                    p => p.AuthorId == userId
                ), productQuery
            );
        }
    }
}

