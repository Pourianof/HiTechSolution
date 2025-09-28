namespace HiTechStore.Data.DTOs.Brand;

public class BrandDto
{
    public int BrandId { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Image { get; set; }
    public IEnumerable<BrandModelDto>? BrandModels { get; set; }
}

public class BrandModelDto
{
    public int? ModelId { get; set; }
    public string? BrandName { get; set; }
    public string? ModelName { get; set; }
    public string? Descriotion { get; set; }
}