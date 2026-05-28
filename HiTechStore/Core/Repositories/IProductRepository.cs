using HiTechStore.Data.DTOs;
using HiTechStore.Data.DTOs.Product;
using HiTechStore.Data.Queries;
using HiTechStore.Models;

namespace HiTechStore.Core.Repositories
{
    public interface IProductRepository : IRepository<Product, ProductDto, ProductQuery>
    {
        Task<ProductDto?> GetByIdAsync(int id, string? userId, ProductQuery? productQuery = default);
        Task<IEnumerable<ProductVariation>> GetAllVariations(IEnumerable<int> variationIds);
        Task<IEnumerable<ProductDto>> GetSimilarProductsOf(int productId, ProductQuery? productQuery = default);
        Task<PagedResultDto<ProductDto>> GetPoductsOfUser(string userId, ProductQuery? productQuery = default);
    }

    public class TimeRange
    {
        public DateTime From { get; set; }
        public DateTime Until { get; set; }
    }
}