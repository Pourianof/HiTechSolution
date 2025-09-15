namespace HiTechStore.Data.DTOs.Product;

public class ProductDto
{
    public int ProductId { get; set; }
    public double Price { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? AuthorId { get; set; }
    public virtual List<ProductMediaDto> Media { get; set; } = new();
    public virtual List<int> Categories { get; set; } = new();
    public double? AverageScore { get; set; } = 0.0;
    public int ScoreCounts { get; set; } = 0;
    public int? MyScore { get; set; }
}