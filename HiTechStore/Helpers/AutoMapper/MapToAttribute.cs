using System;

using AutoMapper;

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
        config.CreateMap(sourceType, Type);
    }
}
