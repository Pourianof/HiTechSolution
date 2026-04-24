using System.Reflection;

namespace HiTechStore.Helpers.AutoMapper;

public class MapFromPropertyAttribute : MapPropertyBaseAttribute
{
    public MapFromPropertyAttribute(string targetPropertyName) : base(targetPropertyName)
    {
    }

    public MapFromPropertyAttribute(string[] path) : base(path)
    {
    }

    public MapFromPropertyAttribute(string targetPropertyName, Type? converter) : base(targetPropertyName, converter)
    {
    }

    public MapFromPropertyAttribute(string? targetPropertyName, Type? converter, string[]? path) : base(targetPropertyName, converter, path)
    {
    }

    protected override string GetDestinationPropertyName(PropertyInfo prop) => prop.Name;

    protected override string GetSourcePropertyName(PropertyInfo prop) => TargetPropertyName!;
}