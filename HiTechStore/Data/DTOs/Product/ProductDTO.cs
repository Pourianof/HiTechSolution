using HiTechStore.Data.DTOs.Brand;
using HiTechStore.Data.DTOs.Component;

namespace HiTechStore.Data.DTOs.Product;

public class ProductDto
{
    public IEnumerable<string>? Inclusions { get; set; }
    public int ProductId { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? AuthorId { get; set; }
    public BrandModelDto? BrandModel { get; set; }
    public List<PropertyValueDto> Properties { get; set; } = new();
    public int? CategoryId { get; set; }
    public List<ProductComponentDto> Components { get; set; } = new();
    public double? AverageScore { get; set; } = 0.0;
    public int ScoreCounts { get; set; } = 0;
    public int? MyScore { get; set; }
    public List<ProductVariationDto> Variations { get; set; } = new();
}

public class ProductComponentDto
{
    public int? ComponentTypeId { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public IEnumerable<ComponentModelDto>? Models { get; set; }
}