using System.Text.Json;

namespace HiTechStore.Presentation.Helpers.Json;


public static class ValueKindExtension
{
    public static object? ConvertJsonElement(this JsonElement element)
    {
        return element.ValueKind switch
        {
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.String => element.GetString(),
            JsonValueKind.Number => element.TryGetInt64(out var i) ? i : element.GetDouble(),
            JsonValueKind.Null => null,
            JsonValueKind.Array => element.EnumerateArray()
                                          .Select(ConvertJsonElement)
                                          .ToList(),
            JsonValueKind.Object => element.EnumerateObject()
                                           .ToDictionary(
                                               p => p.Name,
                                               p => ConvertJsonElement(p.Value)),
            _ => null
        };
    }
}