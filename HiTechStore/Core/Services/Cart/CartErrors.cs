using HiTechStore.Core.Dto.Cart;
using HiTechStore.Core.Helpers.Result;
using HiTechStore.Infrastructure.Data.DTOs;

namespace HiTechStore.Core.Services.Cart;

public static class CartErrors
{
    public static ValidationResultError OutOfAmount(int itemIndex, int requested, int available) =>
        new("Not available", $"Requested amount of items({requested}) is more than available({available}) amounts.", "UnAvailableCartItemAmount", string.Join('.', [
            nameof(CartDto.Items), itemIndex, nameof(CartItemDto.Amount)
        ]));

    public static ValidationResultError NotFoundProduct(int itemIndex) =>
    new("Product not found", $"Specified product id does not exist", "ProductNotFound", string.Join('.', [
        nameof(CartDto.Items), itemIndex, nameof(CartItemDto.ProductVariationId)
    ]));
}