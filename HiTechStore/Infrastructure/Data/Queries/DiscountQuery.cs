namespace HiTechStore.Infrastructure.Data.Queries;

public class DiscountQuery : BaseQuery
{
    public DiscountType? DiscountType { get; set; }
}

public enum DiscountType
{
    All,
    Codes,
    Products
}