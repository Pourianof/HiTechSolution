using HiTechStore.Infrastructure.Data.DTOs.Brand;
using HiTechStore.Core.Models;

namespace HiTechStore.Core.Common.Interfaces.Infra.Repositories;

public interface IBrandModelRepository : IRepository<BrandModel, BrandModelDto>
{
    Task<IEnumerable<BrandModelDto>> GetModelsOfSingleBrand(int brandId);
}