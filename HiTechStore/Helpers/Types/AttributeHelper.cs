using System.Reflection;

namespace HiTechStore.Helpers.Types;

public static class AttributeHelper
{
    public static bool HasGenericAttribute(this PropertyInfo property, Type genericAttributeType)
    {

        return property.GetAttribute(genericAttributeType) is not null;
    }

    public static object? GetAttribute(this PropertyInfo property, Type genericAttributeType)
    {
        var attrs = property.GetCustomAttributes(true);

        foreach (var attr in attrs)
        {
            var attrType = attr.GetType();

            if (attrType.IsGenericType &&
                attrType.GetGenericTypeDefinition() == genericAttributeType)
            {
                return attr;
            }
        }

        return default;
    }

    public static bool TryToGetAttribute(this PropertyInfo property, Type genericAttributeType, out object attr)
    {
        var attrObj = property.GetAttribute(genericAttributeType);

        if (attrObj is not null)
        {
            attr = attrObj;
            return true;
        }
        attr = default!;
        return false;
    }

    public static object[] GetGenericAttributes(PropertyInfo property, Type genericAttributeType)
    {
        var attrs = property.GetCustomAttributes(true);
        var result = new List<object>();

        foreach (var attr in attrs)
        {
            var attrType = attr.GetType();

            if (attrType.IsGenericType &&
                attrType.GetGenericTypeDefinition() == genericAttributeType)
            {
                result.Add(attr);
            }
        }

        return result.ToArray();
    }

    public static TInterface[] GetInterfaceAttributes<TInterface>(this PropertyInfo property)
    {
        var attrs = property.GetCustomAttributes(true);
        return attrs.OfType<TInterface>().ToArray();
    }
}