
using AutoMapper;

using HiTechStore.Core.Repositories;
using HiTechStore.Models;

using Microsoft.EntityFrameworkCore;

namespace HiTechStore.Data.Repositories;

public class CartRepository : Repository<Cart>, ICartRepository
{
    public CartRepository(HiTechStoreDbContext context, IMapper mapper) : base(context, mapper)
    {
    }

    public async Task<Cart?> GetUserActiveCartAsync(string userId)
    {
        return await _dbSet.Where(c => c.Client!.Id == userId).FirstOrDefaultAsync();
    }
}