using HiTechStore.Helpers.URLFilterQuery;
using HiTechStore.Infrastructure.Data.Queries;

namespace HiTechStore.Core.Dto.UserNotification;

public class NotificationQuery : BaseQuery
{
    public QueryFilterItem? CreatedAt { get; set; }
    public QueryFilterItem? State { get; set; } // read, unread, all
}