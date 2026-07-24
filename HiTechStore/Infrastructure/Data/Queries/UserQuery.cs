using HiTechStore.Helpers.URLFilterQuery;

namespace HiTechStore.Infrastructure.Data.Queries;

public class UserQuery : BaseQuery
{
    public QueryFilterItem? Username { get; set; }
    public QueryFilterItem? Id { get; set; }
    public QueryFilterItem? Email { get; set; }

}