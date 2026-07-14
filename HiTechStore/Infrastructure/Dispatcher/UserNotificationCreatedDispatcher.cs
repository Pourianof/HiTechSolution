using HiTechStore.Core.Common.Events;
using HiTechStore.Core.Common.Interfaces.Infra.Repositories;
using HiTechStore.Core.Models;
using HiTechStore.Presentation.RealTime;

using Microsoft.AspNetCore.SignalR;


namespace HiTechStore.Infrastructure.Dispatcher;


public class UserNotificationCreatedDispatcher(
    IHubContext<NotificationHub> hubContext,
    IUserNotificationRepository notificationRepository
) : OutboxDispatcher<UserNotificationCreatedEvent>
{
    public override async Task DispatchAsync(
        UserNotificationCreatedEvent @event,
        CancellationToken cancellationToken)
    {
        var notification = await notificationRepository.GetByIdProjectedAsync(@event.NotificationId);
        if (notification is null)
        {
            return;
        }

        // dispatch notification if target user is connected
        await hubContext.Clients
               .User(notification.OwnerId!)
               .SendAsync(
                   nameof(UserNotification),
                   notification,
                   cancellationToken
               );
    }
}