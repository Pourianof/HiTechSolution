using System.Reflection;

using AutoMapper;

namespace HiTechStore.Helpers.AutoMapper;

public class MapUsingAttribute<TConverter> : Attribute, IMapToConfigAttribute
{
    public void Config(IMappingExpression mappingConfig, PropertyInfo propertyInfo)
    {
        var mapUsingGenericType = typeof(TConverter);

        mappingConfig.ConvertUsing(mapUsingGenericType);
    }
}