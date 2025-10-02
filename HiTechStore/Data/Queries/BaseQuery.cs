using HiTechStore.Helpers.URLFilterQuery;

namespace HiTechStore.Data.Queries;

public class BaseQuery
{
    public QueryFilterItem<int>? Limit { get; set; }
    public QueryFilterItem<int>? Page { get; set; }
}