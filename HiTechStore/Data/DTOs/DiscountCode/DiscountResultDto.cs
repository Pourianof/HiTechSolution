namespace HiTechStore.Data.DTOs.DiscountCode;

public class DiscountResultDto
{
    public string? DiscountCode { get; set; }
    public bool IsDiscountAppliable { get; set; }
    public string? AppliedTo { get; set; }
    public IEnumerable<ProductVariationDto>? DiscountedProducts { get; set; }
    public DiscountActionDto? Discount { get; set; }
}


public enum DiscountTarget
{
    Products,
    Cart
}