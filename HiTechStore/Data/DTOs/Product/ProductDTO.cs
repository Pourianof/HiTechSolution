using HiTechStore.Data.DTOs.Component;

namespace HiTechStore.Data.DTOs.Product;

public class ProductDto
{
    public int ProductId { get; set; }
    public double Price { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? AuthorId { get; set; }
    public List<ProductPropertyValueDto> Properties { get; set; } = new();
    public List<ProductMediaDto> Media { get; set; } = new();
    public int? Category { get; set; }
    public List<ComponentModelDto> Components { get; set; } = new();
    public double? AverageScore { get; set; } = 0.0;
    public int ScoreCounts { get; set; } = 0;
    public int? MyScore { get; set; }
}

public class ProductPropertyValueDto
{
    public string? Value { get; set; }
    public string? Name { get; set; }
    public int PropertyId { get; set; }
}