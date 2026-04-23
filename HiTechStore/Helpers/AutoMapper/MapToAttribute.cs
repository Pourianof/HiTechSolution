namespace HiTechStore.Helpers.AutoMapper;

public class MapToAttribute<T> : MapperAttribute
{
    public Type Type { get; private set; }
    public MapToAttribute()
    {
        Type = typeof(T);
    }

    protected override Type GetSourceType(Type targetType)
    {
        return targetType;
    }

    protected override Type GetDestType(Type targetType)
    {
        return Type;
    }
}
