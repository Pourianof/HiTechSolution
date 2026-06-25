using HiTechStore.Helpers.URLFilterQuery;

namespace HiTechStore.Infrastructure.Data.Queries;

public class UserQuery : BaseQuery
{
    public QueryFilterItem? Username { get; set; }
}