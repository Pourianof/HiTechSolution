namespace HiTechStore.Helpers.URLFilterQuery;

public class Queries
{
    private Dictionary<string, QueryFilterItem> _queries { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    public void Register(QueryFilterItem queryFilterItem)
    {
        _queries.Add(queryFilterItem.Name, queryFilterItem);
    }

    public object? MapTo(Type targetType)
    {
        var markerType = typeof(IQueryFilterItemMarker);

        var props = targetType.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        var obj = targetType.GetConstructors().FirstOrDefault()?.Invoke(null);

        if (obj is null)
        {
            return obj;
        }

        foreach (var prop in props)
        {
            var propName = prop.Name;
            var propType = prop.PropertyType;

            if (_queries.ContainsKey(propName) && markerType.IsAssignableFrom(propType))
            {
                var genericType = propType.GetGenericArguments().FirstOrDefault();
                object? queryFilterItem = _queries[propName];

                if (genericType is not null)
                {
                    var constructor = propType.GetConstructors().FirstOrDefault();
                    var baseQueryFilterItem = (QueryFilterItem)queryFilterItem;
                    queryFilterItem = constructor?.Invoke([baseQueryFilterItem.Name, baseQueryFilterItem.Value, baseQueryFilterItem.Op]);
                }
                prop.SetValue(obj, queryFilterItem);
            }
        }

        return obj;
    }

    public T? MapTo<T>()
    {
        var targetType = typeof(T);
        return (T?)MapTo(targetType);
    }

}
