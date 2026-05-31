using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace HiTechStore.Infrastructure.Data.DTOs.JsonConverters;

public class UnixDateTimeConverter : JsonConverter<DateTime>
{

    private DateTime HandleTimeStampToDateTime(long timestamp)
    {
        if (timestamp > 9999999999)
            return DateTimeOffset.FromUnixTimeMilliseconds(timestamp).UtcDateTime;

        return DateTimeOffset.FromUnixTimeSeconds(timestamp).UtcDateTime;
    }

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number)
        {
            var timestamp = reader.GetInt64();

            return HandleTimeStampToDateTime(timestamp);
        }

        if (reader.TokenType == JsonTokenType.String)
        {
            var value = reader.GetString();

            if (long.TryParse(value, out var ts))
            {
                return HandleTimeStampToDateTime(ts);
            }

            if (DateTime.TryParse(value, CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                out var date))
            {
                return date;
            }
        }

        throw new JsonException("Invalid date format.");
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        var timestamp = new DateTimeOffset(value).ToUnixTimeSeconds();
        writer.WriteNumberValue(timestamp);
    }
}
