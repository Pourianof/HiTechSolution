using HiTechStore.Data.Queries;

namespace HiTechStore.Core.Services.Product;


public static class ProductsDefaultQuery
{
    public static readonly ProductQuery Query = new()
    {
        Limit = 10,
        Page = 1,
    };
}