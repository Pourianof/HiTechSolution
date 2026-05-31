using HiTechStore.Helpers.URLFilterQuery;

namespace HiTechStore.Infrastructure.Data.Queries;

public class BaseQuery
{
    public QueryFilterItem? Limit { get; set; }
    public QueryFilterItem? Page { get; set; }
    public QueryFilterItem? SortBy { get; set; }
    public QueryFilterItem? SortDir { get; set; }

    [MiscQueryFiltersMarker]
    public Dictionary<string, QueryFilterItem> FilterMaps { get; set; } = new();
}

public static class BaseQueryHelper
{
    public static int GetPage(this BaseQuery query)
    {
        var page = query.Page?.GetValue<int>(QueryOperator.Equal) ?? 0;

        return page;
    }
}