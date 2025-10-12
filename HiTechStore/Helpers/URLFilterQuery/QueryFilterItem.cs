namespace HiTechStore.Helpers.URLFilterQuery;

using System.Collections;
using System.Diagnostics;

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
    public void AddOperatorValuePair(QueryOperator @operator, StringValues value)
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
    }

    public IEnumerable<OperatorValuePair> AllFilters => _opValuePairs.Values;
    public IEnumerator GetEnumerator()
    {
        return _opValuePairs.GetEnumerator();
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

                var elementType = targetType.GetGenericArguments()[0];

                var converted = value
                    .Select(v => System.Convert.ChangeType(v, elementType))
                    .ToList();

                if (targetType.IsArray)
                {
                    var array = Array.CreateInstance(elementType, converted.Count);
                    converted.ToArray().CopyTo(array, 0);
                    return array;
                }
                else
                {
                    var listType = typeof(List<>).MakeGenericType(elementType);
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

            return System.Convert.ChangeType(actualValue, targetType);
        }
        catch (Exception)
        {
            return default;
        }
    }
}