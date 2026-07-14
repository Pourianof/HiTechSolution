using HiTechStore.Core.Dto.UserNotification;
using HiTechStore.Core.Models;

namespace HiTechStore.Core.Services.Notification;

public interface INotificationService
{
    Task SyncNotifications();
    Task<IEnumerable<UserNotificationDto>> GetUnreadNotifications();
    Task<UserNotification> CreateNotification(CreateNotificationDto notificationDto);
}