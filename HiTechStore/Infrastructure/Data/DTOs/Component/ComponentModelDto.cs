using HiTechStore.Infrastructure.Data.DTOs.Brand;

namespace HiTechStore.Infrastructure.Data.DTOs.Component;

public class ComponentTypeDto
{
    public int? ComponentTypeId { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public IEnumerable<PropertyDto>? Properties { get; set; }

}

public class ComponentModelDto
{
    public int ComponentModelId { get; set; }
    public int? ComponentTypeId { get; set; }
    public BrandModelDto? BrandModel { get; set; }
    public string? Description { get; set; }
    public IEnumerable<PropertyValueDto>? Properties { get; set; }
}