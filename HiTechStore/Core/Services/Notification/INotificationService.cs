using HiTechStore.Core.Dto.UserNotification;
using HiTechStore.Core.Models;
using HiTechStore.Infrastructure.Data.DTOs;

namespace HiTechStore.Core.Services.Notification;

public interface INotificationService
{
    Task SyncNotifications();
    Task<PagedResultDto<UserNotificationDto>> GetNotifications(string userId, NotificationQuery query);
    Task<UserNotification> CreateNotification(CreateNotificationDto notificationDto);
}