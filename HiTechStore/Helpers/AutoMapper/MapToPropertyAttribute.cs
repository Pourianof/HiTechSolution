using System.Reflection;

namespace HiTechStore.Helpers.AutoMapper;

public class MapToPropertyAttribute : MapPropertyBaseAttribute
{
    public MapToPropertyAttribute(string targetPropertyName) : base(targetPropertyName)
    {
    }

    public MapToPropertyAttribute(string[] path) : base(path)
    {
    }

    public MapToPropertyAttribute(string targetPropertyName, Type? converter) : base(targetPropertyName, converter)
    {
    }

    public MapToPropertyAttribute(string? targetPropertyName, Type? converter, string[]? path) : base(targetPropertyName, converter, path)
    {
    }

    protected override string GetDestinationPropertyName(PropertyInfo prop) => TargetPropertyName!;

    protected override string GetSourcePropertyName(PropertyInfo prop) => prop.Name;
}