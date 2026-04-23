using System.Reflection;

namespace HiTechStore.Helpers.AutoMapper;

public class MapToPropertyAttribute : MapPropertyBaseAttribute
{
    public MapToPropertyAttribute(string targetPropertyName) : base(targetPropertyName)
    {
    }

    public MapToPropertyAttribute(string targetPropertyName, Type? converter) : base(targetPropertyName, converter)
    {
    }

    protected override string GetDestinationPropertyName(PropertyInfo prop) => TargetPropertyName!;

    protected override string GetSourcePropertyName(PropertyInfo prop) => prop.Name;
}