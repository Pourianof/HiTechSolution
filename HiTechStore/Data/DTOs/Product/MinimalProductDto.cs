using HiTechStore.Data.DTOs.Product;

public class MinimalProductDto
{
    public int ProductId { get; set; }
    public double Price { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public List<ProductMediaDto> Media { get; set; } = new();
}
