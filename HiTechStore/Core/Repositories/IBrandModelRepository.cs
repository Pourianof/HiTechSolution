using HiTechStore.Data.DTOs.Brand;
using HiTechStore.Models;

namespace HiTechStore.Core.Repositories;

public interface IBrandModelRepository : IRepository<BrandModel, BrandModelDto>
{
    Task<IEnumerable<BrandModelDto>> GetModelsOfSingleBrand(int brandId);
}