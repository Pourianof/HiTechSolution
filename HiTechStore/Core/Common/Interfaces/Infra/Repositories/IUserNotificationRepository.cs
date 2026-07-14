using HiTechStore.Core.Dto.UserNotification;
using HiTechStore.Core.Models;

namespace HiTechStore.Core.Common.Interfaces.Infra.Repositories;

public interface IUserNotificationRepository : IRepository<UserNotification, UserNotificationDto, Guid>
{
    Task<IEnumerable<UserNotificationDto>> GetUnreadNotifications(string userId);
}