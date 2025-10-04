namespace HiTechStore.Helpers.URLFilterQuery;


using Microsoft.Extensions.Primitives;

public interface IQueryFilterItemMarker { }

public class QueryFilterItem<TValue>(string Name, StringValues Value, QueryOperator Op)
    : IQueryFilterItemMarker
{
    public string Name { get; } = Name;
    public StringValues _val { get; } = Value;
    public TValue? Value
    {
        get
        {
            return QueryFilterItemHelper.Convert<TValue>(_val);
        }
    }
    public QueryOperator Op { get; } = Op;

    public TTarget? GetValue<TTarget>()
    {
        return QueryFilterItemHelper.Convert<TTarget>(_val);
    }

    public void Deconstruct(out string name, out string? value, out QueryOperator op)
    {
        name = Name;
        value = _val;
        op = Op;
    }

}

public class QueryFilterItem
        : QueryFilterItem<StringValues>
{
    public QueryFilterItem(string name, StringValues value, QueryOperator op)
        : base(name, value, op) { }
}

static class QueryFilterItemHelper
{
    public static TTarget? Convert<TTarget>(StringValues value)
    {
        var currentValueType = value.GetType();
        var targetType = typeof(TTarget);

        if (targetType == currentValueType)
        {
            return (TTarget)(object)value;
        }
        try
        {
            var enumerableType = typeof(IEnumerable<>);

            if (enumerableType.IsAssignableFrom(targetType))
            {
                var actualType = targetType.GetGenericArguments().FirstOrDefault();
                if (actualType is null)
                {
                    return default;
                }

                return (TTarget)value.Select(
                    (v) => System.Convert.ChangeType(value, actualType)
                );
            }



            var actualValue = value.FirstOrDefault();
            if (actualValue is null)
            {
                return default;
            }

            return (TTarget)System.Convert.ChangeType(actualValue, targetType);
        }
        catch (Exception)
        {
            return default;
        }
    }
}