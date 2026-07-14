using HiTechStore.Core.Common.Events;
using HiTechStore.Core.Services.Notification;

namespace HiTechStore.Infrastructure.Dispatcher;

public class PermissionChangeUserNotificationHandler(INotificationService notificationService) : OutboxDispatcher<PermissionChangedEvent>
{
    public override async Task DispatchAsync(PermissionChangedEvent @event, CancellationToken cancellationToken)
    {
        await notificationService.CreateNotification(
            new()
            {
                Title = "Permission changed",
                Body = "Your permission and resource access has changed",
                ForUserId = @event.TargetUserId,
                NotificationType = @event.EventName
            }
        );
    }
}