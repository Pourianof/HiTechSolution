using AutoMapper.Internal;

using Castle.Core.Internal;
namespace HiTechStore.Helpers.URLFilterQuery;

public class Queries
{
    private Dictionary<string, QueryFilterItem> _queries { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    public void Register(QueryFilterItem queryFilterItem)
    {
        _queries.Add($"{queryFilterItem.Name}_{queryFilterItem.Op}", queryFilterItem);
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

        List<string> unMatchedKeys = new();

        var namespacedProperties = props.Where((prop) => prop.Has<NamespacedQueryFiltersMarkerAttribute>())
             .Select(
                 (prop) => new
                 {
                     Property = prop,
                     prop.GetAttribute<NamespacedQueryFiltersMarkerAttribute>().Namespace,
                     Storage = new Dictionary<string, QueryFilterItem>()
                 }
             ).ToList();

        foreach (var (key, val) in _queries)
        {
            var matchedProp = props.FirstOrDefault((prop) => string.Equals(prop.Name, val.Name, StringComparison.OrdinalIgnoreCase)
                                                                && markerType.IsAssignableFrom(prop.PropertyType));
            var associatedFilter = _queries[key];

            if (matchedProp is not null)
            {
                var propName = matchedProp.Name;
                var propType = matchedProp.PropertyType;
                var genericType = propType.GetGenericArguments().FirstOrDefault();
                object? queryFilterItem = _queries[key];

                if (genericType is not null)
                {
                    var constructor = propType.GetConstructors().FirstOrDefault();
                    var baseQueryFilterItem = val;
                    queryFilterItem = constructor?.Invoke([baseQueryFilterItem.Name, baseQueryFilterItem.Value, baseQueryFilterItem.Op]);
                }
                matchedProp.SetValue(obj, queryFilterItem);
                continue;
            }

            // Populate namespaced properties
            var matchedNamespacedProperty = namespacedProperties.FirstOrDefault(
                (nsp) => val.Name.StartsWith($"{nsp.Namespace}.", StringComparison.OrdinalIgnoreCase)
            );

            if (matchedNamespacedProperty is not null)
            {
                var scopedKey = val.Name.Substring(matchedNamespacedProperty.Namespace.Length + 1);
                var filter = new QueryFilterItem(
                    scopedKey, associatedFilter.Value, associatedFilter.Op
                );
                matchedNamespacedProperty.Storage.Add(
                    scopedKey, filter
                );
                continue;
            }

            unMatchedKeys.Add(key);
        }

        foreach (var nsp in namespacedProperties)
        {
            if (nsp.Storage.Count() == 0)
            {
                continue;
            }

            nsp.Property.SetValue(
                obj, nsp.Storage
            );
        }

        if (unMatchedKeys.Count() > 0)
        {

            var miscQueryFiltersProperty = props.FirstOrDefault(
                 (prop) => prop.GetAttribute<MiscQueryFiltersMarkerAttribute>() is not null
             );

            if (miscQueryFiltersProperty is not null)
            {
                var remainedFilters = _queries.Where((filter) => unMatchedKeys.Any(umk => umk == filter.Key)).ToDictionary();
                miscQueryFiltersProperty?.SetValue(obj, remainedFilters);
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
