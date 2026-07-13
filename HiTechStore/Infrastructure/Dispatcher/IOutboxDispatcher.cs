using HiTechStore.Core.Common.Events;

namespace HiTechStore.Infrastructure.Dispatcher;

public interface IOutboxDispatcher
{
    Type EventType { get; }

    Task DispatchAsync(
        IEvent @event,
        CancellationToken cancellationToken);
}

public interface IOutboxDispatcher<TEvent> : IOutboxDispatcher
    where TEvent : IEvent
{
    Task DispatchAsync(
        TEvent @event,
        CancellationToken cancellationToken);
}

public abstract class OutboxDispatcher<TEvent>
    : IOutboxDispatcher<TEvent>
    where TEvent : IEvent
{
    public Type EventType => typeof(TEvent);

    public abstract Task DispatchAsync(
        TEvent @event,
        CancellationToken cancellationToken);

    async Task IOutboxDispatcher.DispatchAsync(
        IEvent @event,
        CancellationToken cancellationToken)
    {
        await DispatchAsync(
            (TEvent)@event,
            cancellationToken);
    }
}