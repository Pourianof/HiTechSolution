
using AutoMapper;

using HiTechStore.Core.Common.Interfaces.Infra.Repositories;
using HiTechStore.Infrastructure.Data.DTOs.Brand;
using HiTechStore.Core.Models;

using Microsoft.EntityFrameworkCore;

namespace HiTechStore.Infrastructure.Data.Repositories;

public class BrandRepository : Repository<Brand, BrandDto>, IBrandRepository
{
    public BrandRepository(HiTechStoreDbContext context, IMapper mapper) : base(context, mapper) { }

    public async Task<Brand?> GetByNameAsync(string name)
    {
        return await _dbSet.Where(
            (b) => EF.Functions.ILike(b.Name!, name)
        ).FirstOrDefaultAsync();
    }

    public Task<BrandDto?> GetByNameProjectedAsync(string name)
    {
        return Project(_dbSet.Where(
            (b) => EF.Functions.ILike(b.Name!, name)
        )).FirstOrDefaultAsync();
    }
}