using HiTechStore.Data.DTOs.Brand;
using HiTechStore.Models;

namespace HiTechStore.Data.DTOs.Component;

public class ComponentTypeDto
{
    public int? ComponentTypeId { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
}

public class ComponentModelDto
{
    public int ComponentModelId { get; set; }
    public virtual ComponentTypeDto? ComponentType { get; set; }
    public virtual BrandModelDto? BrandModel { get; set; }
    public string? Description { get; set; }
    public virtual IEnumerable<ComponentPropertyValue>? Properties { get; set; }
}