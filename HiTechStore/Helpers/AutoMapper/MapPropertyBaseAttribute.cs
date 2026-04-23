using System.Reflection;

using AutoMapper;

namespace HiTechStore.Helpers.AutoMapper;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public abstract class MapPropertyBaseAttribute : Attribute, IMapConfigAttribute
{
    public string? TargetPropertyName { get; set; }
    public Type? Converter { get; set; }
    public MapPropertyBaseAttribute(string targetPropertyName) : this(targetPropertyName, default)
    { }

    public MapPropertyBaseAttribute(string targetPropertyName, Type? converter)
    {
        TargetPropertyName = targetPropertyName;
        Converter = converter;
    }

    private bool ConverterSpecified => Converter is not null;

    protected abstract string GetSourcePropertyName(PropertyInfo prop);
    protected abstract string GetDestinationPropertyName(PropertyInfo prop);

    public void Config(IMappingExpression mappingConfig, PropertyInfo prop)
    {
        var sourcePropertyName = GetSourcePropertyName(prop);
        mappingConfig.ForMember(
            GetDestinationPropertyName(prop),
            opt =>
            {
                opt.MapFrom(sourcePropertyName);
                if (ConverterSpecified)
                {
                    opt.ConvertUsing(Converter, sourcePropertyName);
                }
            }
        );
    }
}