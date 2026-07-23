
using AutoMapper;

using HiTechStore.Core.Common.Interfaces.Infra.Repositories;
using HiTechStore.Core.Dto.UserNotification;
using HiTechStore.Core.Models;
using HiTechStore.Helpers.URLFilterQuery;
using HiTechStore.Helpers.URLFilterQuery.QueryAppliers;
using HiTechStore.Infrastructure.Data.DTOs;

using Microsoft.EntityFrameworkCore;


namespace HiTechStore.Infrastructure.Data.Repositories;

public class UserNotificationRepository : Repository<UserNotification, UserNotificationDto, NotificationQuery, Guid>, IUserNotificationRepository
{
    public UserNotificationRepository(HiTechStoreDbContext context, IMapper mapper) : base(context, mapper)
    {
    }

    protected override IQueryable<UserNotification> GetAllQueryBuilder(IQueryable<UserNotification> queryBuilder, NotificationQuery? queyParams = null)
    {
        var createdAtFilters = queyParams?.CreatedAt?.GetFilters(
            QueryOperator.GreaterThan |
            QueryOperator.GreaterThanOrEqual |
            QueryOperator.LessThan |
            QueryOperator.GreaterThanOrEqual
        );

        if (createdAtFilters is not null)
        {
            queryBuilder = queryBuilder.ApplyFiltersTo<UserNotification, DateTime>(
                createdAtFilters,
                new SinglePropertyQueryApplier<UserNotification, DateTime>(
                    un => un.CreatedAt
                )
            );
        }

        var typeFilters = queyParams?.Type?.GetFilters(
            QueryOperator.In |
            QueryOperator.Nin |
            QueryOperator.Equal
        );

        if (typeFilters is not null)
        {
            queryBuilder = queryBuilder.ApplyFiltersTo<UserNotification, string>(
                typeFilters,
                new SinglePropertyQueryApplier<UserNotification, string>(
                    un => un.Type!
                )
            );
        }

        queryBuilder = queryBuilder.OrderBy(un => un.CreatedAt).OrderDescending();

        return queryBuilder;
    }

    public async Task<PagedResultDto<UserNotificationDto>> GetUsersNotifications(string userId, NotificationQuery query)
    {
        var state = query.State?.GetValue<string>(QueryOperator.Equal);

        var efQuery = _dbSet.Where(
            un => un.OwnerId == userId
        );

        if (!string.IsNullOrEmpty(state))
        {
            efQuery = state.ToLower() switch
            {
                "unread" => efQuery.Where(un => un.ReadAt == null),
                "read" => efQuery.Where(un => un.ReadAt != null),
                _ => efQuery
            };
        }

        return await GetPagedResult<UserNotificationDto>(efQuery, query);
    }

    public Task DeleteNotificationsBefore(DateTime until)
    {
        return _dbSet.Where(
            un => un.CreatedAt <= until
        ).ExecuteUpdateAsync(setter =>
            setter.SetProperty(
                un => un.IsDeleted,
                true
            )
        );
    }
}