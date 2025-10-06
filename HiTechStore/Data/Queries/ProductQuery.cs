using HiTechStore.Helpers.URLFilterQuery;

namespace HiTechStore.Data.Queries;

public class ProductQuery : BaseQuery
{
    public QueryFilterItem<int>? Category { get; set; }
    public QueryFilterItem<string>? Color { get; set; }
    public QueryFilterItem<string>? Brand { get; set; }
    [NamespacedQueryFiltersMarker("ct")]
    public Dictionary<string, QueryFilterItem>? CategoryProperties { get; set; }
}