using AutoMapper.QueryableExtensions;

using HiTechStore.Core.Repositories;
using HiTechStore.Models;

using Microsoft.EntityFrameworkCore;

namespace HiTechStore.Data.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(HiTechStoreDbContext context) : base(context)
        {

        }

        private IQueryable<Product> BaseGettingQuery(IQueryable<Product> queryBuilder)
        {
            return queryBuilder
                .Include((p) => p.Media).Include(p => p.Properties).ThenInclude(pp => pp.Property)
                .Select(p => new Product
                {
                    ProductId = p.ProductId,
                    Title = p.Title,
                    AverageScore = p.Scores.Any()
                                 ? p.Scores.Average(s => (double?)s.Score)
                                 : 0.0,
                    ScoreCounts = p.Scores.Count(),
                    AuthorId = p.AuthorId,
                    Description = p.Description,
                    Price = p.Price,
                    Media = p.Media,
                    Properties = p.Properties
                });
        }

        protected override IQueryable<Product> GetAllQueryBuilder(IQueryable<Product> queryBuilder)
        {
            return BaseGettingQuery(queryBuilder);
        }

        protected override IQueryable<Product> GetByIdAsyncQueryBuilder(IQueryable<Product> queryBuilder)
        {
            return BaseGettingQuery(queryBuilder);
        }

        public async Task<Product?> GetByIdAsync(int id, string? userId)
        {
            return await BaseGettingQuery(_dbSet).FirstOrDefaultAsync();

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
