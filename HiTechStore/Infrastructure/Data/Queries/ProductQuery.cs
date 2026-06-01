using HiTechStore.Helpers.URLFilterQuery;

namespace HiTechStore.Infrastructure.Data.Queries;

public class ProductQuery : BaseQuery
{
    public QueryFilterItem? Category { get; set; }
    public QueryFilterItem? Color { get; set; }
    public QueryFilterItem? Brand { get; set; }
    public QueryFilterItem? Price { get; set; }
    public QueryFilterItem? Include { get; set; }
    public QueryFilterItem? SearchTerm { get; set; }
    // [BindingQuery]
    // public BestSellerQuery? BestSeller { get; set; }
    public QueryFilterItem? BestSeller { get; set; }
    public Dictionary<string, QueryFilterItem>? CategoryProperties { get; set; }
    public QueryFilterItem? Discount { get; set; }

    public ProductQuery CopyWith(ProductQuery? query)
    {
        return new()
        {
            Brand = query?.Brand ?? Brand,
            Category = query?.Category ?? Category,
            Color = query?.Color ?? Color,
            Discount = query?.Discount ?? Discount,
            Limit = query?.Limit ?? Limit,
            Page = query?.Page ?? Page,
            Price = query?.Price ?? Price,
            SortBy = query?.SortBy ?? SortBy,
            SortDir = query?.SortDir ?? SortDir,
            Include = query?.Include ?? Include,
            SearchTerm = query?.SearchTerm ?? SearchTerm
        };
    }
}

// public class BestSellerQuery
// {
//     public QueryFilterItem? From { get; set; }
//     public QueryFilterItem? Until { get; set; }
// }