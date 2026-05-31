using HiTechStore.Infrastructure.Data.DTOs.Discount;
using HiTechStore.Infrastructure.Data.Queries;
using HiTechStore.Core.Models;

namespace HiTechStore.Core.Common.Interfaces.Infra.Repositories;

public interface IDiscountCodeRepository : IRepository<Discount, DiscountDto, DiscountQuery>
{
    public Task<IEnumerable<Discount?>> GetDiscountCodeByNameAsync(string name);
    public Task<IEnumerable<DiscountDto?>> GetDiscountCodeByNameProjectedAsync(string name);
    public Task<IEnumerable<Discount>> GetActiveDiscountsOfTypeAsync(DiscountType discountType = DiscountType.All);
}
