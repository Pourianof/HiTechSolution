
using AutoMapper;

using HiTechStore.Core.Repositories;
using HiTechStore.Data.DTOs.Brand;
using HiTechStore.Models;

using Microsoft.EntityFrameworkCore;

namespace HiTechStore.Data.Repositories;

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