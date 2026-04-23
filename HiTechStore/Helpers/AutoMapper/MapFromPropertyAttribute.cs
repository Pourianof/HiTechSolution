using System.Reflection;

namespace HiTechStore.Helpers.AutoMapper;

public class MapFromPropertyAttribute : MapPropertyBaseAttribute
{
    public MapFromPropertyAttribute(string targetPropertyName) : base(targetPropertyName)
    {
    }

    public MapFromPropertyAttribute(string targetPropertyName, Type? converter) : base(targetPropertyName, converter)
    {
    }

    protected override string GetDestinationPropertyName(PropertyInfo prop) => prop.Name;

    protected override string GetSourcePropertyName(PropertyInfo prop) => TargetPropertyName!;
}