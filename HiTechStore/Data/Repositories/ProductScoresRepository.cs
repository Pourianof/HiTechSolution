
using AutoMapper;

using HiTechStore.Core.Repositories;
using HiTechStore.Models;

using Microsoft.EntityFrameworkCore;

namespace HiTechStore.Data.Repositories;

public class ProductScoresRepository : Repository<ProductScore>, IProductScoresRepository
{


    public ProductScoresRepository(HiTechStoreDbContext context, IMapper mapper)
    : base(context, mapper)
    {

    }

    public Task<ProductScore?> GetUserScoreForProductAsync(string userId, int productId)
    {
        return _dbSet.Where(ps => ps.UserId == userId && ps.ProductId == productId)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<ProductScore>> GetUserScoresAsync(string userId)
    {
        return await _dbSet
            .Where(ps => ps.UserId == userId)
            .ToListAsync();
    }
}