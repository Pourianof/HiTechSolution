using HiTechStore.Models;

namespace HiTechStore.Core.Services.Discount;

public abstract class DiscountTargetResource
{
    required public DiscountAction? DiscountAction { get; init; }
    public DiscountTargetResource(DiscountAction discountAction)
    {
        DiscountAction = discountAction;
    }
}


public class CartDiscount : DiscountTargetResource
{
    public CartDiscount(DiscountAction discountAction) : base(discountAction)
    {
    }
}


public class ProductDiscount : DiscountTargetResource
{
    public Product Product { get; init; }
    public ProductDiscount(Product product, DiscountAction discountAction) : base(discountAction)
    {
        Product = product;
    }
}
