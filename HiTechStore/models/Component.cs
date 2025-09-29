using HiTechStore.Core;

namespace HiTechStore.Models;

public class ComponentType : IModel
{
    public int ComponentTypeId { get; set; }
    public string? Name { get; set; }
    public virtual IEnumerable<CategoryComponent>? Categories { get; set; }
    public virtual List<Property>? Properties { get; set; }
    public virtual List<ComponentModel>? ComponentModels { get; set; }
    public string? Description { get; set; }
}


public class ComponentModel : IModel
{
    public int ComponentModelId { get; set; }
    public virtual int? ComponentTypeId { get; set; }
    public virtual ComponentType? ComponentType { get; set; }
    public virtual BrandModel? BrandModel { get; set; }
    public string? Description { get; set; }
    public virtual IEnumerable<ComponentPropertyValue>? Properties { get; set; }
}

public class ComponentPropertyValue
{
    public int ComponentPropertyValueId { get; set; }
    public virtual PropertyValue? Value { get; set; }
    public int PropertyId { get; set; }
    public virtual Property? Property { get; set; }
    public virtual ComponentModel? ComponentModel { get; set; }
}
