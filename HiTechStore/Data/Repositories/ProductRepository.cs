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
        public override async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _dbSet.Select(p => new Product
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
            }).ToListAsync();

        }

        public override async Task<Product?> GetByIdAsync(int id)
        {
            return await _dbSet.Where(p => p.ProductId == id).Select(p => new Product
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
            }).FirstOrDefaultAsync();
        }

        public async Task<Product?> GetByIdAsync(int id, string? userId)
        {
            return await _dbSet.Where(p => p.ProductId == id).Select(p => new Product
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
                MyScore = userId != null ? p.Scores.Where((s) => s.ProductId == id && s.UserId == userId).Select((s) => s.Score).Single() : null
            }).FirstOrDefaultAsync();
        }
    }
}
