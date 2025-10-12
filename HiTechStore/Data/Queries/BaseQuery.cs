using HiTechStore.Helpers.URLFilterQuery;

namespace HiTechStore.Data.Queries;

public class BaseQuery
{
    public QueryFilterItem? Limit { get; set; }
    public QueryFilterItem? Page { get; set; }
    [MiscQueryFiltersMarker]
    public Dictionary<string, QueryFilterItem> FilterMaps { get; set; } = new();
}