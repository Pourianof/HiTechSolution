using HiTechStore.Models;

namespace HiTechStore.Core.Repositories;

public interface IDiscountCodeRepository : IRepository<DiscountCode>
{
    public Task<DiscountCode?> GetDiscountCodeByNameAsync(string name);
}
