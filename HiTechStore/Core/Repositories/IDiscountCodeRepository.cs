using HiTechStore.Data.DTOs.DiscountCode;
using HiTechStore.Models;

namespace HiTechStore.Core.Repositories;

public interface IDiscountCodeRepository : IRepository<DiscountCode, DiscountCodeDto>
{
    public Task<DiscountCode?> GetDiscountCodeByNameAsync(string name);
    public Task<DiscountCodeDto?> GetDiscountCodeByNameProjectedAsync(string name);
}
