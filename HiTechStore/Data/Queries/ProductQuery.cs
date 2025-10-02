using HiTechStore.Helpers.URLFilterQuery;

namespace HiTechStore.Data.Queries;

public class ProductQuery : BaseQuery
{
    public QueryFilterItem<int>? Category { get; set; }
    // public 
}