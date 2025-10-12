namespace HiTechStore.Helpers.Types;

public static class EnumerableExtensions
{
    public static IEnumerable<TSource> WhereNotNull<TSource>(this IEnumerable<TSource> vals)
    {
        return vals.Where((val) => val is not null);
    }
}