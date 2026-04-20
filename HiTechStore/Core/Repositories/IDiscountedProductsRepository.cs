using HiTechStore.Data.DTOs.Product;
using HiTechStore.Data.Queries;
using HiTechStore.Models;

namespace HiTechStore.Core.Repositories;

public interface IDiscountedProductsRepository
{
    Task<IEnumerable<ProductDto>> GetDiscountedProducts(IEnumerable<DiscountRule> rules, ProductQuery? productQuery = default);
    Task<IEnumerable<ProductDto>> GetProductsByCondition(ConditionComponent conditionTree);
}
