namespace HiTechStore.Helpers.URLFilterQuery;

[Flags]
public enum QueryOperator
{
    Equal = 0,
    GreaterThan = 1 << 0,
    GreaterThanOrEqual = 1 << 1,
    LessThan = 1 << 2,
    LessThanOrEqual = 1 << 3,
    In = 1 << 4,
    Nin = 1 << 5
}