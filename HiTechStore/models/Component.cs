namespace HiTechStore.Models;

public class ComponentType
{
    public int ComponentTypeId { get; set; }
    public string? Name { get; set; }
    public virtual IEnumerable<Category>? Categories { get; set; }
    public virtual IEnumerable<Property>? Properties { get; set; }
    public string? Description { get; set; }
}


public class ComponentModel
{
    public int ComponentModelId { get; set; }
    public virtual ComponentType? ComponentType { get; set; }
    public virtual BrandModel? BrandModel { get; set; }
    public string? Description { get; set; }
}

public class ComponentPropertyValue
{
    public int ComponentPropertyValueId { get; set; }
    public virtual Property? Property { get; set; }
    public virtual ComponentModel? ComponentModel { get; set; }
}
