using HiTechStore.Data.DTOs.Brand;
using HiTechStore.Models;

namespace HiTechStore.Core.Repositories;

public interface IBrandRepository : IRepository<Brand, BrandDto>
{
    Task<BrandDto?> GetByNameProjectedAsync(string name);
    Task<Brand?> GetByNameAsync(string name);
}