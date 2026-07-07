using AutoMapper;

using HiTechStore.Core.Common.Interfaces.Infra.Repositories;
using HiTechStore.Infrastructure.Data.DTOs.Brand;
using HiTechStore.Core.Models;

using Microsoft.EntityFrameworkCore;

namespace HiTechStore.Infrastructure.Data.Repositories;

public class BrandModelRepository : RepositoryWithIntegerId<BrandModel, BrandModelDto>, IBrandModelRepository
{
    public BrandModelRepository(HiTechStoreDbContext context, IMapper mapper) : base(context, mapper) { }

    public async Task<IEnumerable<BrandModelDto>> GetModelsOfSingleBrand(int brandId)
    {
        return await Project(_context.BrandModel.Where(
            bm => bm.Brand!.BrandId == brandId
        )).ToListAsync();
    }

}