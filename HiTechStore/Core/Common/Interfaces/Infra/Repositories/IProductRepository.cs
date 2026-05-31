using HiTechStore.Infrastructure.Data.DTOs;
using HiTechStore.Infrastructure.Data.DTOs.Product;
using HiTechStore.Infrastructure.Data.Queries;
using HiTechStore.Core.Models;

namespace HiTechStore.Core.Common.Interfaces.Infra.Repositories
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