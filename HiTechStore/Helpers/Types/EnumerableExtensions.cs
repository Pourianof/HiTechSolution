namespace HiTechStore.Helpers.Types;

public static class EnumerableExtensions
{
    public static IEnumerable<TSource> WhereNotNull<TSource>(this IEnumerable<TSource?> vals)
        where TSource : class
    {
        return vals.Where((val) => val is not null)!;
    }

    public static IEnumerable<TSource> WhereNotNull<TSource>(this IEnumerable<TSource?> vals)
       where TSource : struct
    {
        return vals.Where((val) => val.HasValue).Select(val => val!.Value);
    }
}