using System.Reflection;

using AutoMapper;

namespace HiTechStore.Helpers.AutoMapper;

public interface IMapToConfigAttribute
{
    void Config(IMappingExpression mappingConfig, PropertyInfo propertyInfo);
}