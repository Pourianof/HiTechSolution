using HiTechStore.Models;

namespace HiTechStore.Core.Services;

public interface IDiscountService
{
    string GenerateRanomCode(int length = 10);
    Task<DiscountCode> RegisterDiscountCode(DiscountCode discountCode);
}