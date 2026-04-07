using System.Reflection;

using AutoMapper;

namespace HiTechStore.Helpers.AutoMapper;

[AttributeUsage(AttributeTargets.Property)]
public class MapToPropertyAttribute : Attribute, IMapToConfigAttribute
{
    public string? TargetPropertyName { get; set; }
    public MapToPropertyAttribute(string targetPropertyName)
    {
        TargetPropertyName = targetPropertyName;
    }

    public void Config(IMappingExpression mappingConfig, PropertyInfo prop)
    {
        if (prop.Name != TargetPropertyName)
        {
            mappingConfig.ForMember(
                TargetPropertyName,
                opt => opt.MapFrom(prop.Name)
            );
        }
    }
}