using System.Text.Json;

using AutoMapper;

using HiTechStore.Presentation.Helpers.Json;

namespace HiTechStore.Presentation.MapConverters;


public class JsonElementToObjectConverter
    : IValueConverter<object?, object?>
{
    public object? Convert(
        object? source,
        ResolutionContext context)
    {
        if (source is JsonElement src)
        {
            return src.ConvertJsonElement();
        }

        return default;
    }
}