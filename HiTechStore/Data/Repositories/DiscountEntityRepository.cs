
using AutoMapper;

using HiTechStore.Core.Repositories;
using HiTechStore.Data.DTOs.DiscountEntity;
using HiTechStore.Models;

using Microsoft.EntityFrameworkCore;

using Npgsql;

namespace HiTechStore.Data.Repositories;

public class DiscountEntityRepository : Repository<DiscountEntity, DiscountEntityDto>, IDiscountEntityRepository
{
    public DiscountEntityRepository(HiTechStoreDbContext context, IMapper mapper) : base(context, mapper)
    {
    }

    public async Task<DiscountEntityProperty?> GetPropertyById(int propertyId)
    {
        return await _context.Set<DiscountEntityProperty>().FirstOrDefaultAsync(
            (p) => p.DiscountEntityPropertyId == propertyId
        );
    }
}
