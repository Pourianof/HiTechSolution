using HiTechStore.Core.Common.Events;
using HiTechStore.Infrastructure.Dispatcher;
using HiTechStore.Presentation.RealTime;

using Microsoft.AspNetCore.SignalR;

public class PermissionChangeDispatcher(
    IHubContext<NotificationHub> hubContext)
    : OutboxDispatcher<PermissionChangedEvent>
{
    public override Task DispatchAsync(
        PermissionChangedEvent @event,
        CancellationToken cancellationToken)
    {
        return hubContext.Clients
            .User(@event.TargetUserId)
            .SendAsync(
                @event.EventName,
                @event,
                cancellationToken);
    }
}