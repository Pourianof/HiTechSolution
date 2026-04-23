using System.Reflection;

using AutoMapper;

namespace HiTechStore.Helpers.AutoMapper;

public interface IMapConfigAttribute
{
    void Config(IMappingExpression mappingConfig, PropertyInfo propertyInfo);
}