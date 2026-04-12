namespace HiTechStore.Helpers.Types;


public static class LongTypeExtensions
{
    public static DateTime ToDateTime(this long timestamp)
    {
        return DateTimeOffset.FromUnixTimeMilliseconds(timestamp).LocalDateTime;
    }
}