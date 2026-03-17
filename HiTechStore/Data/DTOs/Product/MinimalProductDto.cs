using HiTechStore.Data.DTOs;

public class MinimalProductDto
{
    public int ProductId { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public IEnumerable<ProductVariationWithCartAmount> Variations { get; set; } = new List<ProductVariationWithCartAmount>();
}

public class ProductVariationWithCartAmount
{
    public int Amount { get; set; }
    public ProductVariationDto? Variation { get; set; }
}