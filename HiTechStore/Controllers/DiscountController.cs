using HiTechStore.Core.Services.Discount;
using HiTechStore.Data.DTOs.Discount;

using Microsoft.AspNetCore.Mvc;

namespace HiTechStore.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class DiscountsController(IDiscountService discountService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<DiscountDto>> RegisterDiscount(DiscountCreationDto discountCreationDto)
    {
        return Ok(await discountService.RegisterDiscount(discountCreationDto));
    }
}