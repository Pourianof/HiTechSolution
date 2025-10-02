namespace HiTechStore.Helpers.URLFilterQuery;

public interface IQueryParser
{
    Queries Parse(IQueryCollection queryParams);
}