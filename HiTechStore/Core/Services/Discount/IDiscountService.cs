using HiTechStore.Data.DTOs;
using HiTechStore.Data.DTOs.Discount;
using HiTechStore.Data.Queries;

namespace HiTechStore.Core.Services.Discount;

public interface IDiscountService
{
    string GenerateRanomCode(int length = 10);
    Task<DiscountDto> RegisterDiscountCode(DiscountCodeCreationDto discountCode);
    Task<DiscountDto> RegisterDiscount(DiscountCreationDto discount);
    Task<PagedResultDto<DiscountDto>> GetAllDiscountCodes(DiscountQuery? query);
    Task<DiscountDto?> UpdateDiscountCode(int id, DiscountCodeUpdateDto discountCodeUpdateDto, System.Security.Claims.ClaimsPrincipal user);
    Task<bool> DeleteDiscountCode(int id);
    // check is discount code usable for current cart/order by user
    Task<DiscountResultDto> CheckDiscountCodeUsability(string discountCode, string userId);
    Task<Models.Discount?> GetActiveDiscountCodeOf(string discountCode);
    Task<ConditionParseResult> GetConditionScriptProducts(string conditionScript);
}