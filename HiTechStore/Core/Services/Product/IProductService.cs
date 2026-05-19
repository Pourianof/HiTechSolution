using HiTechStore.Data.DTOs;
using HiTechStore.Data.DTOs.Product;
using HiTechStore.Data.Queries;
using HiTechStore.DTOs.Product;

namespace HiTechStore.Core.Services.Product;

public interface IProductService
{
    Task<PagedResultDto<ProductDto>> GetProducts(ProductQuery query);
    Task<Models.Product?> DeleteProduct(int id);
    Task<ProductDto> CreateProduct(ProductCreationDto product, string userId);
    Task<PagedResultDto<ProductDto>> GetOnSaleProducts();
    Task<ProductDto?> GetProductById(int product, ProductAccessAdditionalProcessing? discountCalculation = default);
    Task<IEnumerable<ProductDto>> GetSimilarProductsOf(int productId);
    Task<PagedResultDto<ProductDto>> GetUsersProducts(ProductQuery? productQuery = default);
    Task<ProductBasicInfoDto> UpdateProduct(int productId, UpdateProductDto? updateDto);
}

public class ProductAccessAdditionalProcessing
{
    public bool DiscountCalculation { get; set; } = false;
    public bool UsersScore { get; set; } = false;
}

public class UpdateProductDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public int? BrandModelId { get; set; }
}