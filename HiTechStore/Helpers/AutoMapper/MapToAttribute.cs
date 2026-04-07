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
            string candidateTargetPropertyName = prop.Name;
            if (prop.TryToGetAttribute(typeof(MapToPropertyAttribute), out var propertyAttr))
            {
                candidateTargetPropertyName = ((MapToPropertyAttribute)propertyAttr).TargetPropertyName!;
            }

            if (prop.Name != candidateTargetPropertyName)
            {
                mapConfig.ForMember(
                    candidateTargetPropertyName,
                    opt => opt.MapFrom(prop.Name)
                );
            }

            if (prop.TryToGetAttribute(typeof(MapUsingAttribute<>), out var attr))
            {
                var mapUsingGenericType = attr.GetType().GenericTypeArguments[0];

                mapConfig.ConvertUsing(mapUsingGenericType);
            }
        }

    }
}
