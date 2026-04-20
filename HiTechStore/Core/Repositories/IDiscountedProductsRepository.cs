using HiTechStore.Data.DTOs;
using HiTechStore.Data.DTOs.Product;
using HiTechStore.Data.Queries;
using HiTechStore.Models;

namespace HiTechStore.Core.Repositories;

public interface IDiscountedProductsRepository
{
    Task<PagedResultDto<ProductDto>> GetDiscountedProducts(IEnumerable<DiscountRule> rules, ProductQuery? productQuery = default);
    Task<IEnumerable<ProductDto>> GetProductsByCondition(ConditionComponent conditionTree);
}
