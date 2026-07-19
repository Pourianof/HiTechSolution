
using System.Text.Json;

using HiTechStore.Core.Common.Events;
using HiTechStore.Core.Common.Interfaces.Infra;
using HiTechStore.Infrastructure.Data.Repositories;

namespace HiTechStore.Infrastructure.Event;

public class EventPublisher(OutboxMessageRepository repo) : IEventPublisher
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
    }
}