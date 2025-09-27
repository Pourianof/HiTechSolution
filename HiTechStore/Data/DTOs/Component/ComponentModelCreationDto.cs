namespace HiTechStore.Data.DTOs.Component;

public class ComponentModelCreationDto
{
    public int? BrandModelId { get; set; }
    public string? Description { get; set; }
    public IEnumerable<PropertyValueDto>? Properties { get; set; }
}