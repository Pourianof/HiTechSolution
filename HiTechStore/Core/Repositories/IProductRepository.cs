using HiTechStore.Data.DTOs.Product;
using HiTechStore.Data.Queries;
using HiTechStore.Models;

namespace HiTechStore.Core.Repositories
{
    public interface IProductRepository : IRepository<Product, ProductDto, ProductQuery>
    {
        Task<ProductDto?> GetByIdAsync(int id, string? userId);
        Task<IEnumerable<ProductVariation>> GetAllVariations(IEnumerable<int> variationIds);
        Task<IEnumerable<ProductDto>> GetDiscountedProducts(ConditionComponent componentTree);
    }
}
