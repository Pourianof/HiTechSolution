using HiTechStore.Data.DTOs.Discount;
using HiTechStore.Data.Queries;
using HiTechStore.Models;

namespace HiTechStore.Core.Repositories;

public interface IDiscountCodeRepository : IRepository<Discount, DiscountDto, DiscountQuery>
{
    public Task<IEnumerable<Discount?>> GetDiscountCodeByNameAsync(string name);
    public Task<IEnumerable<DiscountDto?>> GetDiscountCodeByNameProjectedAsync(string name);
    public Task<IEnumerable<Discount>> GetActiveDiscountsOfTypeAsync(DiscountType discountType = DiscountType.All);
}
