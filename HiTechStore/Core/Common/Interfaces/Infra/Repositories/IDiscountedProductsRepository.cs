using HiTechStore.Infrastructure.Data.DTOs;
using HiTechStore.Infrastructure.Data.DTOs.Product;
using HiTechStore.Infrastructure.Data.Queries;
using HiTechStore.Core.Models;

namespace HiTechStore.Core.Common.Interfaces.Infra.Repositories;

public interface IDiscountedProductsRepository
{
    Task<PagedResultDto<ProductDto>> GetDiscountedProducts(IEnumerable<DiscountRule> rules, ProductQuery? productQuery = default);
    Task<IEnumerable<ProductDto>> GetProductsByCondition(ConditionComponent conditionTree);
}
