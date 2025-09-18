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

        protected override IQueryable<Product> GetAllQueryBuilder(IQueryable<Product> queryBuilder)
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
                Description = p.Description,
                Price = p.Price,
                Media = p.Media
            });
        }

        protected override IQueryable<Product> GetByIdAsyncQueryBuilder(IQueryable<Product> queryBuilder)
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
                Description = p.Description,
                Price = p.Price,
                Media = p.Media,
            });
        }

        public async Task<Product?> GetByIdAsync(int id, string? userId)
        {
            return await _dbSet.Include((p) => p.Media).Where(p => p.ProductId == id).Select(p => new Product
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
                MyScore = userId != null ? p.Scores.Where((s) => s.ProductId == id && s.UserId == userId).Select((s) => s.Score).Single() : null,
                Media = p.Media,
            }).FirstOrDefaultAsync();

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
