namespace HiTechStore.Helpers.Types;

public static class EnumberableHelpers
{
    public static bool TryGetEnumerableItemType(Type type, out Type itemType)
    {
        itemType = null!;

        if (type.IsArray)
        {
            itemType = type.GetElementType()!;
            return true;
        }

        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
        {
            itemType = type.GetGenericArguments()[0];
            return true;
        }

        var ienum = type.GetInterfaces()
                        .FirstOrDefault(i =>
                            i.IsGenericType &&
                            i.GetGenericTypeDefinition() == typeof(IEnumerable<>));

        if (ienum != null)
        {
            itemType = ienum.GetGenericArguments()[0];
            return true;
        }

        return false;
    }
}