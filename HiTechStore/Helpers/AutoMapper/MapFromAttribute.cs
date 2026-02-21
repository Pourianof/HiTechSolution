using System;

using AutoMapper;

namespace HiTechStore.Helpers.AutoMapper;

public class MapFromAttribute<T> : MapperAttribute
{
    public Type Type { get; private set; }
    public MapFromAttribute()
    {
        Type = typeof(T);
    }

    public override void Map(IMapperConfigurationExpression config, Type sourceType)
    {
        config.CreateMap(Type, sourceType);
    }
}
