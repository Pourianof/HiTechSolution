namespace HiTechStore.Helpers.URLFilterQuery;

using System.Collections;
using System.Diagnostics;
using System.Numerics;

using Microsoft.Extensions.Primitives;

public interface IQueryFilterItemMarker { }

public class QueryFilterItem
    : IQueryFilterItemMarker, IEnumerable
{
    public string FilterKey { get; init; }
    private Dictionary<QueryOperator, OperatorValuePair> _opValuePairs = new(6);
    public QueryFilterItem(string Name)
    {
        FilterKey = Name;
    }

    public IEnumerable<TValue?> GetValues<TValue>(QueryOperator op)
    {
        var results = new List<TValue?>();
        foreach (var (key, filter) in _opValuePairs)
        {
            if (op.HasFlag(key))
            {
                var operatorValue = filter.GetValue<IEnumerable<TValue>>();
                if (operatorValue is not null)
                {
                    results.AddRange(operatorValue);
                }

            }
        }

        return results;
    }

    public TValue? GetValue<TValue>(QueryOperator op)
    {
        if (_opValuePairs.TryGetValue(op, out var filter))
        {
            return filter.GetValue<TValue>();
        }
        return default;
    }

    public Dictionary<QueryOperator, OperatorValuePair> GetFilters(QueryOperator queryOperator)
    {
        Dictionary<QueryOperator, OperatorValuePair> filters = new();
        foreach (var (key, filter) in _opValuePairs)
        {
            if (queryOperator.HasFlag(key))
            {
                filters.Add(key, filter);
            }
        }

        return filters;
    }
    public QueryFilterItem AddOperatorValuePair(QueryOperator @operator, StringValues value)
    {
        if (_opValuePairs.ContainsKey(@operator))
        {
            // Note: In ASP.NET request query analyser, combine all same key queries
            // as a single StringValues type value, so this branch not executed at all
            // but for integrability and compatibility with other systems we put this 
            // feature
            var pair = _opValuePairs[@operator];
            pair.HandleNewValue(value);
        }
        else
        {
            Func<QueryOperator, OperatorValuePair> opMap = (QueryOperator op) => op switch
            {
                QueryOperator.Equal => new EqualityOperatorPair(value),
                QueryOperator.In => new InOperatorPair(value),
                QueryOperator.GreaterThan => new GreaterThanOperatorPair(value),
                QueryOperator.GreaterThanOrEqual => new GreaterThanOrEqualOperatorPair(value),
                QueryOperator.LessThan => new LessThanOperatorPair(value),
                QueryOperator.LessThanOrEqual => new LessThanOrEqualOperatorPair(value),
                _ => throw new UnreachableException()
            };
            _opValuePairs.Add(
                @operator,
                opMap(@operator)
            );
        }

        return this;
    }

    public IEnumerable<OperatorValuePair> AllFilters => _opValuePairs.Values;
    public IEnumerator GetEnumerator()
    {
        return _opValuePairs.GetEnumerator();
    }
    public static QueryFilterItem From(object? value)
    {
        return CreateDefaultItem(ToStringValues(value));
    }

    public static implicit operator QueryFilterItem(int value)
        => From(value);
    public static implicit operator QueryFilterItem(double value)
        => From(value);

    public static implicit operator QueryFilterItem(string value)
        => CreateDefaultItem(new StringValues(value));

    public static implicit operator QueryFilterItem(string[] value)
        => CreateDefaultItem(new StringValues(value));

    public static implicit operator QueryFilterItem(StringValues value)
        => CreateDefaultItem(value);

    private static StringValues ToStringValues(object? value)
    {
        if (value is null)
            return StringValues.Empty;

        if (value is StringValues sv)
            return sv;

        if (value is string s)
            return new StringValues(s);

        if (value is IEnumerable<string> strEnum)
            return new StringValues(strEnum.ToArray());

        if (value is IEnumerable enumerable and not string)
        {
            var arr = enumerable
                .Cast<object?>()
                .Select(x => x?.ToString() ?? string.Empty)
                .ToArray();

            return new StringValues(arr);
        }

        return new StringValues(value.ToString());
    }

    private static QueryFilterItem CreateDefaultItem(StringValues values)
    {
        return new QueryFilterItem("no-key")
            .AddOperatorValuePair(QueryOperator.Equal, values);
    }
}

static class QueryFilterItemHelper
{
    public static TTarget? Convert<TTarget>(StringValues value)
    {
        var targetType = typeof(TTarget);
        try
        {
            return (TTarget?)Convert(value, targetType);
        }
        catch (Exception)
        {
            return default;
        }
    }
    public static object? Convert(StringValues value, Type targetType)
    {
        object? ChangeType(object? value, Type targetType)
        {
            var finalValue = System.Convert.ChangeType(value, targetType);

            // TODO: It is only a temporary modification to fix Entity Framework
            // UTC(Time-Zone) compability. 
            // I know this code don't belong to this method so i must change it
            if (finalValue is DateTime fv)
            {
                return fv.ToUniversalTime();
            }

            return finalValue;
        }
        ;

        var currentValueType = value.GetType();

        if (targetType == currentValueType)
        {
            return value;
        }
        try
        {
            var enumerableType = typeof(IEnumerable);

            if (enumerableType.IsAssignableFrom(targetType))
            {
                if (targetType == typeof(string))
                {
                    return value.FirstOrDefault();
                }
                var actualType = targetType.GetGenericArguments().FirstOrDefault();
                if (actualType is null)
                {
                    return default;
                }

                var converted = value
                    .Select(v => ChangeType(v, actualType))
                    .ToList();

                if (targetType.IsArray)
                {
                    var array = Array.CreateInstance(actualType, converted.Count);
                    converted.ToArray().CopyTo(array, 0);
                    return array;
                }
                else
                {
                    var listType = typeof(List<>).MakeGenericType(actualType);
                    var list = (IList?)Activator.CreateInstance(listType);
                    if (list is null)
                    {
                        return default;
                    }
                    foreach (var item in converted)
                    {
                        list.Add(item);
                    }
                    return list;
                }
            }



            var actualValue = value.FirstOrDefault();
            if (actualValue is null)
            {
                return default;
            }

            return ChangeType(actualValue, targetType);
        }
        catch (Exception)
        {
            return default;
        }
    }
}