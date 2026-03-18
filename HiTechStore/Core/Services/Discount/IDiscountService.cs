using HiTechStore.Data.DTOs;
using HiTechStore.Data.DTOs.DiscountCode;
using HiTechStore.Data.Queries;
using HiTechStore.Models;

namespace HiTechStore.Core.Services.Discount;

public interface IDiscountService
{
    string GenerateRanomCode(int length = 10);
    Task<DiscountCode> RegisterDiscountCode(DiscountCode discountCode);
    Task<PagedResultDto<DiscountCodeDto>> GetAllDiscountCodes(DiscountQuery? query);
    Task<DiscountCodeDto?> UpdateDiscountCode(int id, DiscountCodeUpdateDto discountCodeUpdateDto, System.Security.Claims.ClaimsPrincipal user);
    Task<bool> DeleteDiscountCode(int id);
    Task<DiscountResultDto> CheckDiscountCodeUsability(string discountCode, string userId);
    Task<DiscountCode?> GetActiveDiscountCodeOf(string discountCode);
}