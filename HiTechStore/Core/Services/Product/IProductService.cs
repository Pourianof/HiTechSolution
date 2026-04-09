using HiTechStore.Data.DTOs;
using HiTechStore.Data.DTOs.Product;
using HiTechStore.Data.Queries;

namespace HiTechStore.Core.Services.Product;

public interface IProductService
{
    Task<PagedResultDto<ProductDto>> GetProducts(ProductQuery query);
}