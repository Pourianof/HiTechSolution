

using System.Security.Claims;

using HiTechStore.Core.Models;
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

    public override async Task OnConnectedAsync()
    {
        if (Context.User?.Identity?.IsAuthenticated != true)
        {
            await base.OnConnectedAsync();
            Context.Abort();
            return;
        }

        var notifications =
            await notificationService.GetUnreadNotifications();

        foreach (var notification in notifications)
        {
            await Clients.Caller.SendAsync(
                nameof(UserNotification),
                notification);
        }

        await base.OnConnectedAsync();
    }
}