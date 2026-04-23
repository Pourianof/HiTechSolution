namespace HiTechStore.Helpers.AutoMapper;

public class MapFromAttribute<T> : MapperAttribute
{
    public Type Type { get; private set; }
    public MapFromAttribute()
    {
        Type = typeof(T);
    }

    protected override Type GetSourceType(Type targetType)
    {
        return Type;
    }

    protected override Type GetDestType(Type targetType)
    {
        return targetType;
    }
}
