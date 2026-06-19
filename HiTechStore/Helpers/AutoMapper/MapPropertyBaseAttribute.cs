using System.Reflection;

using AutoMapper;

namespace HiTechStore.Helpers.AutoMapper;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public abstract class MapPropertyBaseAttribute : Attribute, IMapConfigAttribute
{
    public string? TargetPropertyName { get; set; }
    public Type? Converter { get; set; }
    public string[]? TargetPropertyPath { get; set; }
    public MapPropertyBaseAttribute(string targetPropertyName) : this(targetPropertyName, default, default)
    { }

    public MapPropertyBaseAttribute(string[] path) : this(default, default, path)
    { }

    public MapPropertyBaseAttribute(string? targetPropertyName, Type? converter, string[]? path)
    {
        TargetPropertyName = targetPropertyName;
        Converter = converter;
        TargetPropertyPath = path;

        if (TargetPropertyPath is null && TargetPropertyName is null)
        {
            throw new InvalidDataException("Both property name and property path are null");
        }

        if (targetPropertyName is null && TargetPropertyPath!.Length == 0)
        {
            throw new InvalidDataException("Property path have no sub-path element");
        }
        else if (targetPropertyName is null)
        {
            TargetPropertyName = string.Join(".", TargetPropertyPath!);
        }
    }

    public MapPropertyBaseAttribute(string targetPropertyName, Type? converter) : this(targetPropertyName)
    {
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