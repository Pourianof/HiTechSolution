using AutoMapper;

namespace HiTechStore.Helpers.AutoMapper;

[AttributeUsage(AttributeTargets.Class)]
public abstract class MapperAttribute : Attribute
{
    abstract public void Map(IMapperConfigurationExpression config, Type sourceType);
}
