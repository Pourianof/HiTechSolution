
using AutoMapper;

using HiTechStore.Core.Repositories;
using HiTechStore.Models;

using Microsoft.EntityFrameworkCore;

namespace HiTechStore.Data.Repositories;

public class DiscountCodeRepository : Repository<DiscountCode>, IDiscountCodeRepository
{
    public DiscountCodeRepository(HiTechStoreDbContext context, IMapper mapper) : base(context, mapper)
    {
    }

    public Task<DiscountCode?> GetDiscountCodeByNameAsync(string name)
    {
        return _dbSet.FirstOrDefaultAsync(
            (code) => EF.Functions.ILike(name, code.Code!)
        );
    }
}
