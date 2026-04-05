using System.Security.Claims;

using HiTechStore.Core;
using HiTechStore.Core.Services.Discount;
using HiTechStore.Data.DTOs;
using HiTechStore.Data.DTOs.Cart;
using HiTechStore.Data.DTOs.Discount;
using HiTechStore.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HiTechStore.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CartsController(IUnitOfWork unitOfWork) : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    /*
        the algorithm is simple.
        always specify the last state of cart items
    */
    [HttpPatch("items")]
    public async Task<IActionResult> SyncCart(CartDto cartDto)
    {
        var specifiedProductIds = cartDto.Items!.Select(i => i.ProductVariationId);
        var addingCartItemProducts = await _unitOfWork.Products.GetAllVariations(specifiedProductIds);

        if (addingCartItemProducts.Count() != specifiedProductIds.Count())
        {
            var notExistedProducts = specifiedProductIds.Select((p, i) => new { ProductId = p, Index = i }).Where(
                indexedProd => !addingCartItemProducts.Any(p => p.Product!.ProductId == indexedProd.ProductId)
            );

            foreach (var product in notExistedProducts)
            {
                ModelState.AddModelError(
                    $"{nameof(CartDto.Items)}.{product.Index}.{nameof(CartItemDto.ProductVariationId)}",
                    "Specified product id does not exist"
                );

                return ValidationProblem(ModelState);
            }
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var cart = await _unitOfWork.CartRepository.GetUserActiveCartAsync(userId);


        // check if user has active cart or not
        if (cart is not null)
        {
            var newProductItems = cartDto.Items!
                .Where(item => !cart.Items.Any(i => i.ProductVariationId == item.ProductVariationId))
                .Select(
                    item => new CartItem()
                    {
                        Amount = item.Amount,
                        ProductVariationId = item.ProductVariationId
                    }
                );

            cart.Items = cart.Items.Select(
                item => new CartItem
                {
                    ProductVariationId = item.ProductVariationId,
                    Amount = cartDto.Items!.FirstOrDefault(i => i.ProductVariationId == item.ProductVariationId)?.Amount ?? item.Amount,
                }
            ).ToList();
            cart.Items.AddRange(newProductItems);

            // remove items with 0 count
            cart.Items = cart.Items.Where(item => item.Amount != 0).ToList();
        }
        else
        {
            cart = new Cart()
            {
                ClientId = userId,
                Items = cartDto.Items!.Select(item => new CartItem()
                {
                    Amount = item.Amount,
                    ProductVariationId = item.ProductVariationId
                }).ToList()
            };
            // create total new cart
            await _unitOfWork.CartRepository.AddAsync(cart);
        }

        await _unitOfWork.Complete();

        // return cart with the products
        return Ok(await _unitOfWork.CartRepository.GetUserActiveCartWithProductAsync(userId));
    }

    public async Task<ActionResult<CartWithProductsDto>> GetUserCart()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var cart = await _unitOfWork.CartRepository.GetUserActiveCartWithProductAsync(userId);

        return Ok(cart ?? new CartWithProductsDto() { Items = [] });

    }

    [HttpGet("discount/state")]
    public async Task<ActionResult<DiscountResultDto>> GetDiscountAppliableState([FromQuery] string discountCode, [FromServices] IDiscountService discountService)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

        return Ok(await discountService.CheckDiscountCodeUsability(discountCode, userId));

    }
}