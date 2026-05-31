using HiTechStore.Infrastructure.Data.DTOs;
using HiTechStore.Infrastructure.Data.DTOs.Brand;

public class ProductBasicInfoDto
{
    public int ProductId { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? AuthorId { get; set; }
    public BrandModelDto? BrandModel { get; set; }
}

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