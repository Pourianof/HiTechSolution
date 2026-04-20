using HiTechStore.Data.DTOs;
using HiTechStore.Data.DTOs.Product;
using HiTechStore.Data.Queries;
using HiTechStore.DTOs.Product;
using HiTechStore.Models;

namespace HiTechStore.Core.Services.Product;

public interface IProductService
{
    Task<PagedResultDto<ProductDto>> GetProducts(ProductQuery query);
    Task<ProductScore> ScoreProduct(int productId, ProductScoreDto score, string userId);
    Task<Models.Product?> DeleteProduct(int id);
    Task<ProductDto> CreateProduct(ProductCreationDto product, string userId);
    Task<PagedResultDto<ProductDto>> GetOnSaleProducts();
}