

using System.Security.Claims;

using HiTechStore.Core.Services.Notification;

using Microsoft.AspNetCore.SignalR;

namespace HiTechStore.Presentation.RealTime;

public class NotificationHub(INotificationService notificationService) : Hub
{
    public static string Route = "/notifications";

    public async Task SyncNotifications()
    {
        var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId is null)
        {
            Context.Abort();
            return;
        }

        await notificationService.SyncNotifications();
    }
}