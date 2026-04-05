using HiTechStore.Data.DTOs.Discount;
using HiTechStore.Models;

namespace HiTechStore.Core.Repositories;

public interface IDiscountCodeRepository : IRepository<Discount, DiscountDto>
{
    public Task<IEnumerable<Discount?>> GetDiscountCodeByNameAsync(string name);
    public Task<IEnumerable<DiscountDto?>> GetDiscountCodeByNameProjectedAsync(string name);
}
