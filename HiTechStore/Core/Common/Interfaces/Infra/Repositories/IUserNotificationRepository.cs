using HiTechStore.Core.Dto.UserNotification;
using HiTechStore.Core.Models;
using HiTechStore.Infrastructure.Data.DTOs;

namespace HiTechStore.Core.Common.Interfaces.Infra.Repositories;

public interface IUserNotificationRepository : IRepository<UserNotification, UserNotificationDto, NotificationQuery, Guid>
{
    Task<PagedResultDto<UserNotificationDto>> GetUsersNotifications(string userId, NotificationQuery query);
    Task DeleteNotificationsBefore(DateTime until);
}