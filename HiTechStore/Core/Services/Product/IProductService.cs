using HiTechStore.Core.Dto.Product;
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
    Task<PagedResultDto<ProductDto>> GetOnSaleProducts(ProductQuery? productQuery = default);
    Task<ProductDto?> GetProductById(int product, ProductAccessAdditionalProcessing? discountCalculation = default, ProductQuery? query = default);
    Task<IEnumerable<ProductDto>> GetSimilarProductsOf(int productId, ProductQuery? productQuery = default);
    Task<PagedResultDto<ProductDto>> GetUsersProducts(ProductQuery? productQuery = default);
    Task<ProductBasicInfoDto> UpdateProduct(int productId, UpdateProductDto? updateDto);
    Task<ProductDto> UpdateProductsCategory(int productId, ProductCategoryValuesDto replaceDto);
}

public class ProductAccessAdditionalProcessing
{
    public bool DiscountCalculation { get; set; } = false;
    public bool UsersScore { get; set; } = false;
}

