using HiTechStore.Core.Models;

namespace HiTechStore.Core.Services.Notification;

public interface INotificationService
{
    Task SyncNotifications();
    Task<UserNotification> CreateNotification();
}