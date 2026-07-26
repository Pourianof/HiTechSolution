using HiTechStore.Core.Helpers.Result;

namespace HiTechStore.Core.Services.Order;

public static class OrderErrors
{
    public static ResultError EmptyCart() => new()
    {
        Title = "No cart exist for payment",
        Description = "No cart with some items exists",
        Code = nameof(EmptyCart)
    };

    public static ValidationResultError DiscountIsNotAppliable(string discountCode) => new()
    {
        Title = "Discount couldn't applied",
        Description = $"Specified discount code({discountCode}) is not appliable to your cart, ensure you are in right place",
        Code = nameof(DiscountIsNotAppliable),
        FieldName = nameof(discountCode)
    };

    public static ValidationResultError OutOfStockItem(
        int productVariationId,
        int inventory,
        int requestedAmount) => new()
        {
            Title = "Not available item",
            Description = $"Specified variation of product item with id \"{productVariationId}\" has not enough inventory({inventory}) to cover your {requestedAmount} request",
            Code = nameof(OutOfStockItem),
            FieldName = $"cart[variationId:{productVariationId}]"
        };

    public static ResultError OrderNotFound(int orderId) => new()
    {
        Title = "Order not found",
        Description = "No order existed associated with verfication-key",
        Code = nameof(OrderNotFound)
    };

    public static ResultError InvalidConfirmationKey() => new()
    {
        Title = "specified verification-key is not a valid key",
        Description = "the payment validation-key is not valid. Contatct with support",
        Code = nameof(InvalidConfirmationKey)
    };

    public static ResultError InvalidPaymentConfirmation() => new()
    {
        Title = "invalid payment verification key",
        Description = "provided verification key not valid",
        Code = nameof(InvalidPaymentConfirmation)
    };

}