using HiTechStore.Infrastructure.Data.DTOs.Brand;
using HiTechStore.Core.Models;

namespace HiTechStore.Core.Common.Interfaces.Infra.Repositories;

public interface IBrandRepository : IRepositoryWithIntegerId<Brand, BrandDto>
{
    Task<BrandDto?> GetByNameProjectedAsync(string name);
    Task<Brand?> GetByNameAsync(string name);
}