using AutoMapper;

using HiTechStore.Core.Repositories;
using HiTechStore.Data.DTOs.Brand;
using HiTechStore.Models;

using Microsoft.EntityFrameworkCore;

namespace HiTechStore.Data.Repositories;

public class BrandModelRepository : Repository<BrandModel, BrandModelDto>, IBrandModelRepository
{
    public BrandModelRepository(HiTechStoreDbContext context, IMapper mapper) : base(context, mapper) { }

    public async Task<IEnumerable<BrandModelDto>> GetModelsOfSingleBrand(int brandId)
    {
        return await Project(_context.BrandModel.Where(
            bm => bm.Brand!.BrandId == brandId
        )).ToListAsync();
    }

}