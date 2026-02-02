using HiTechStore.Data.DTOs;
using HiTechStore.Data.DTOs.Product;

public class MinimalProductDto
{
    public int ProductId { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public List<ProductVariationDto> Variations { get; set; } = new();
}
