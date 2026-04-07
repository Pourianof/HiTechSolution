using AutoMapper;

using HiTechStore.Helpers.Types;

namespace HiTechStore.Helpers.AutoMapper;

public class MapToAttribute<T> : MapperAttribute
{
    public Type Type { get; private set; }
    public MapToAttribute()
    {
        Type = typeof(T);
    }

    public override void Map(IMapperConfigurationExpression config, Type sourceType)
    {
        var sourceProperties = sourceType.GetProperties();

        var mapConfig = config.CreateMap(sourceType, Type);

        foreach (var prop in sourceProperties)
        {
            var configAttributes = prop.GetInterfaceAttributes<IMapToConfigAttribute>();

            foreach (var configAttr in configAttributes)
            {
                configAttr.Config(mapConfig, prop);
            }
        }

    }
}
