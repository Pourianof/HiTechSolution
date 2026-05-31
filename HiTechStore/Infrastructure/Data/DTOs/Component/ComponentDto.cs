namespace HiTechStore.Infrastructure.Data.DTOs.Component;


public class ComponentTypeWithPropertiesDto
{
    public int? ComponentTypeId { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public IEnumerable<PropertyDto>? Properties { get; set; }
}