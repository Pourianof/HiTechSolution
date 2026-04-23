using AutoMapper;

using HiTechStore.Helpers.Types;

namespace HiTechStore.Helpers.AutoMapper;

[AttributeUsage(AttributeTargets.Class)]
public abstract class MapperAttribute : Attribute
{
    protected abstract Type GetSourceType(Type targetType);
    protected abstract Type GetDestType(Type targetType);

    public void Map(IMapperConfigurationExpression config, Type sourceType)
    {
        var sourceProperties = sourceType.GetProperties();

        var mapConfig = config.CreateMap(GetSourceType(sourceType), GetDestType(sourceType));

        foreach (var prop in sourceProperties)
        {
            var configAttributes = prop.GetInterfaceAttributes<IMapConfigAttribute>();

            foreach (var configAttr in configAttributes)
            {
                configAttr.Config(mapConfig, prop);
            }
        }

    }
}
