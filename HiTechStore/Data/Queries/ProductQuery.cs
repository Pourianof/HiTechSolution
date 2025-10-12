using HiTechStore.Helpers.URLFilterQuery;

namespace HiTechStore.Data.Queries;

public class ProductQuery : BaseQuery
{
    public QueryFilterItem? Category { get; set; }
    public QueryFilterItem? Color { get; set; }
    public QueryFilterItem? Brand { get; set; }
    public QueryFilterItem? Price { get; set; }
    [NamespacedQueryFiltersMarker("ct")]
    public Dictionary<string, QueryFilterItem>? CategoryProperties { get; set; }
    public QueryFilterItem? Discount { get; set; }
}