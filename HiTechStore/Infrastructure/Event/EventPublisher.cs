
using System.Text.Json;

using HiTechStore.Core.Common.Events;
using HiTechStore.Core.Common.Interfaces.Infra;
using HiTechStore.Infrastructure.Data.Repositories;
using HiTechStore.Infrastructure.Helpers;

namespace HiTechStore.Infrastructure.Event;

public class EventPublisher(OutboxMessageRepository repo, OutboxSignal outboxSignal) : IEventPublisher
{
    public async Task PublishAsync(IEvent @event)
    {
        await repo.AddAsync(
            new()
            {
                EventType = @event.GetType().Name,
                Payload = JsonSerializer.Serialize(@event, @event.GetType())
            }
        );

        await repo.Complete();

        outboxSignal.Notify();
    }
}