using System.Reflection;

using AutoMapper.Internal;

using Castle.Core.Internal;

using Microsoft.Extensions.Primitives;
namespace HiTechStore.Helpers.URLFilterQuery;

public class Queries
{
    private Dictionary<string, QueryFilterItem> _queries { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    public void Register(string keyName, QueryOperator op, StringValues value)
    {
        var isKeyExist = _queries.ContainsKey(keyName);
        QueryFilterItem filter = isKeyExist ?
                _queries[keyName] : new QueryFilterItem(keyName);

        filter.AddOperatorValuePair(op, value);
        if (!isKeyExist)
        {
            _queries.Add(keyName, filter);
        }
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

        var bindingProperties = props.Where(prop => prop.Has<BindingQueryAttribute>())
           .Select(
               (prop) => new BindingPropertyContainer()
               {
                   Property = prop,
                   PropName = prop.Name,
                   PropType = prop.PropertyType
               }
           ).ToList();

        foreach (var (key, val) in _queries)
        {
            var matchedProp = props.FirstOrDefault((prop) => string.Equals(prop.Name, val.FilterKey, StringComparison.OrdinalIgnoreCase)
                                                                && markerType.IsAssignableFrom(prop.PropertyType));
            var associatedFilter = _queries[key];

            if (matchedProp is not null)
            {
                matchedProp.SetValue(obj, val);
                continue;
            }

            // Populate namespaced properties
            var matchedNamespacedProperty = namespacedProperties.FirstOrDefault(
                (nsp) => val.FilterKey.StartsWith($"{nsp.Namespace}.", StringComparison.OrdinalIgnoreCase)
            );

            if (matchedNamespacedProperty is not null)
            {
                var scopedKey = val.FilterKey.Substring(matchedNamespacedProperty.Namespace.Length + 1);
                var filter = new QueryFilterItem(scopedKey);
                matchedNamespacedProperty.Storage.Add(
                    scopedKey, filter
                );
                continue;
            }

            var matchedBindedProperty = bindingProperties.FirstOrDefault(
                (bp) => val.FilterKey.StartsWith($"{bp.Property.Name}.", StringComparison.OrdinalIgnoreCase)
            );

            if (matchedBindedProperty is not null)
            {
                var bindingField = val.FilterKey.Substring(matchedBindedProperty.PropName.Length + 1);
                var boundedProp = matchedBindedProperty.PropType.GetProperties().FirstOrDefault(bindingProp => string.Equals(bindingProp.Name, bindingField, StringComparison.OrdinalIgnoreCase));

                if (boundedProp is not null)
                {
                    if (boundedProp.GetType() == typeof(QueryFilterItem))
                    {
                        // for now we just populate one level binding
                        // maybe on next updates we populate nested scenario recursively
                        var filter = new QueryFilterItem(bindingField);
                        boundedProp.SetValue(matchedBindedProperty.BindingModel, filter);
                        continue;
                    }
                }
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

        foreach (var bp in bindingProperties)
        {

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


public class BindingPropertyContainer
{
    required public PropertyInfo Property { get; set; }
    required public string PropName { get; set; }
    required public Type PropType { get; set; }
    private object? _bindingModel = default;
    public object BindingModel
    {
        get
        {
            return _bindingModel ??= PropType.GetConstructors().FirstOrDefault()!.Invoke(null);
        }
    }
}