using System.Security.Claims;

using HiTechStore.Core.Services.Discount;
using HiTechStore.Infrastructure.Data.DTOs;
using HiTechStore.Core.Dto.Cart;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HiTechStore.Core.Dto.Discount;
using HiTechStore.Core.Services.Cart;

namespace HiTechStore.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CartsController(ICartService cartService) : AppControllerBase
{

    /*
        the algorithm is simple.
        always specify the last state of cart items
    */
    [HttpPatch("items")]
    public async Task<IActionResult> SyncCart(CartDto cartDto)
    {
        var finalCartResult = await cartService.SyncCart(cartDto);

        // return cart with the products
        return ResultCheck(finalCartResult);
    }

    public async Task<ActionResult<CartWithProductsDto>> GetUserCart()
    {
        var cartResult = await cartService.GetUserCart();

        return ResultCheck(cartResult);
    }

    [HttpGet("discount/state")]
    public async Task<ActionResult<DiscountResultDto>> GetDiscountAppliableState([FromQuery] string discountCode, [FromServices] IDiscountService discountService)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

        return Ok(await discountService.CheckDiscountCodeUsability(discountCode, userId));

    }
}