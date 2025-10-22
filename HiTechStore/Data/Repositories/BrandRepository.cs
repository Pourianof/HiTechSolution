
using AutoMapper;

using HiTechStore.Core.Repositories;
using HiTechStore.Data.DTOs.Brand;
using HiTechStore.Models;

using Microsoft.EntityFrameworkCore;

namespace HiTechStore.Data.Repositories;

public class BrandRepository : Repository<Brand, BrandDto>, IBrandRepository
{
    public BrandRepository(HiTechStoreDbContext context, IMapper mapper) : base(context, mapper) { }

    public Task<BrandDto?> GetBrandByName(string name)
    {
        return Project(_dbSet.Where(
            (b) => EF.Functions.Like(b.Name, name)
        )).FirstOrDefaultAsync();
    }
}