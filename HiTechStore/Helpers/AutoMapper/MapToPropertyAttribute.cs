using System.Reflection;

using AutoMapper;

namespace HiTechStore.Helpers.AutoMapper;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public class MapToPropertyAttribute : Attribute, IMapToConfigAttribute
{
    public string? TargetPropertyName { get; set; }
    public Type? Converter { get; set; }
    public MapToPropertyAttribute(string targetPropertyName) : this(targetPropertyName, default)
    { }

    public MapToPropertyAttribute(string targetPropertyName, Type? converter)
    {
        TargetPropertyName = targetPropertyName;
        Converter = converter;
    }

    private bool ConverterSpecified => Converter is not null;

    public void Config(IMappingExpression mappingConfig, PropertyInfo prop)
    {
        mappingConfig.ForMember(
            TargetPropertyName,
            opt =>
            {
                opt.MapFrom(prop.Name);
                if (ConverterSpecified)
                {
                    opt.ConvertUsing(Converter, prop.Name);
                }
            }
        );
    }
}