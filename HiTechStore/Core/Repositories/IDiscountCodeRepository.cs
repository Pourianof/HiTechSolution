using HiTechStore.Data.DTOs.DiscountCode;
using HiTechStore.Models;

namespace HiTechStore.Core.Repositories;

public interface IDiscountCodeRepository : IRepository<DiscountCode, DiscountCodeDto>
{
    public Task<IEnumerable<DiscountCode?>> GetDiscountCodeByNameAsync(string name);
    public Task<IEnumerable<DiscountCodeDto?>> GetDiscountCodeByNameProjectedAsync(string name);
}
