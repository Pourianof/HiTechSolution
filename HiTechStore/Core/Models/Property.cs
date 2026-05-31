namespace HiTechStore.Core.Models;

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
    public virtual IEnumerable<ComponentPropertyValue>? ComponentValues { get; set; }
    public virtual IEnumerable<ProductPropertyValue>? ProductValues { get; set; }
}

public class PropertyValue
{
    public int PropertyValueId { get; set; }
    public int? ComponentPropertyValueId { get; set; }
    private string? _stringValue;
    public string? ValueString
    {
        get
        {
            return ValueNumber?.ToString() ??
                    ValueBoolean?.ToString() ??
                    ValueDateTime?.ToString() ??
                    _stringValue;
        }
        set => _stringValue = value;
    }
    public double? ValueNumber { get; set; }
    public bool? ValueBoolean { get; set; }
    public DateTime? ValueDateTime { get; set; }
    public int? ValueReferenceId { get; set; }
}


public class BaseItemPropertyValue
{
    public virtual PropertyValue? Value { get; set; }
    public int PropertyId { get; set; }
    public virtual Property? Property { get; set; }
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
