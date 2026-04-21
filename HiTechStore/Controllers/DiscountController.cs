using HiTechStore.Core.Services.Discount;
using HiTechStore.Data.DTOs.Discount;
using HiTechStore.Data.Queries;
using HiTechStore.Helpers.URLFilterQuery;
using HiTechStore.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HiTechStore.Controllers;

[ApiController]
[Route("/api/[controller]")]
[Authorize]
public class DiscountsController(IDiscountService discountService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<DiscountDto>> RegisterDiscount(DiscountCreationDto discountCreationDto)
    {
        return Ok(await discountService.RegisterDiscount(discountCreationDto));
    }

    [HttpGet]
    public async Task<ActionResult<Discount>> GetDiscounts([ToQuery] DiscountQuery discountQuery, [FromQuery] DiscountType? discountType)
    {
        if (discountType is not null)
        {
            discountQuery.DiscountType = discountType;
        }

        return Ok(await discountService.GetDiscounts(discountQuery));
    }
}