using System.Reflection;

using AutoMapper;

namespace HiTechStore.Helpers.AutoMapper;

[AttributeUsage(AttributeTargets.Property)]
public class MapIgnore : Attribute, IMapConfigAttribute
{
    public void Config(IMappingExpression mappingConfig, PropertyInfo propertyInfo)
    {
        mappingConfig.ForMember(
            propertyInfo.Name,
            (opt) => opt.Ignore()
        );
    }
}