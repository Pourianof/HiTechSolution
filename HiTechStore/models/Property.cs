namespace HiTechStore.Models;

public enum PropertyType
{
    String = 0,
    Number,
    Boolean,
    DateTime,
    Reference
}

public class Property
{
    public int PropertyId { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Unit { get; set; }
    public virtual PropertyType PropertyType { get; set; }
}

public class PropertyValue
{
    public int PropertyValueId { get; set; }
    public int ComponentPropertyValueId { get; set; }
    public string? ValueString { get; set; }
    public double? ValueNumber { get; set; }
    public bool? ValueBoolean { get; set; }
    public DateTime? ValueDateTime { get; set; }
    public int? ValueReferenceId { get; set; }
}

public static class PropertyTypeHelper
{
    public static string GetNameOfCategoryPropertyType(PropertyType type) => type switch
    {
        PropertyType.Number => "Number",
        PropertyType.String => "String",
        PropertyType.Boolean => "Boolean",
        PropertyType.Reference => "Reference",
        PropertyType.DateTime => "DateTime",
        _ => "Not defined"
    };
}
