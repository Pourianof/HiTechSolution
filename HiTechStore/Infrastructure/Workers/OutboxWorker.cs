using System.Text.Json;

using HiTechStore.Core.Common.Events;
using HiTechStore.Infrastructure.Data.Repositories;
using HiTechStore.Infrastructure.Dispatcher;
using HiTechStore.Infrastructure.Helpers;

namespace HiTechStore.Infrastructure.Workers;

public sealed class OutboxWorker(
    OutboxSignal signal,
    IServiceProvider serviceProvider,
    ILogger<OutboxWorker> logger,
    EventTypeResolver eventTypeResolver)
    : BackgroundService
{
    private readonly OutboxSignal _signal = signal;
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly ILogger<OutboxWorker> _logger = logger;
    private readonly EventTypeResolver _eventTypeResolver = eventTypeResolver;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var signalTask = _signal.WaitAsync(stoppingToken);
                var timeoutTask = Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);

                await Task.WhenAny(signalTask, timeoutTask);

                await ProcessPendingMessages(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Outbox worker failed.");

                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }

    private async Task ProcessPendingMessages(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();

        var repo = scope.ServiceProvider.GetRequiredService<OutboxMessageRepository>();

        var messages = await repo.GetUnprocessedMessages();

        foreach (var message in messages)
        {
            await ProcessMessage(message.Id, cancellationToken);
        }
    }

    private async Task ProcessMessage(
        Guid messageId,
        CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();

        var repo = scope.ServiceProvider.GetRequiredService<OutboxMessageRepository>();
        var dispatcherRegistry = scope.ServiceProvider.GetRequiredService<IOutboxDispatcherRegistry>();

        var message = await repo.GetModelByIdAsync(messageId);

        if (message is null)
            return;

        var eventType = _eventTypeResolver.Resolve(message.EventType);
        if (eventType is null)
        {
            throw new InvalidOperationException(
                $"No event-type could find with name {message.EventType}"
            );
        }

        var handlers = dispatcherRegistry.GetDispatchers(eventType);

        if (handlers.Count == 0)
        {
            _logger.LogWarning(
                "No dispatcher found for event '{EventType}'",
                message.EventType);

            return;
        }

        try
        {
            if (message.Payload is not null)
            {
                var eventObject = JsonSerializer.Deserialize(message.Payload, eventType) as IEvent;
                if (eventObject is null)
                {
                    throw new InvalidOperationException(
                        $"Could not map message payload to event object for message with id {message.Id}"
                    );
                }

                await Task.WhenAll(
                    handlers.Select(x =>
                        x.DispatchAsync(eventObject, cancellationToken)));
            }

            message.ProcessedAt = DateTime.UtcNow;
            await repo.Complete();

        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed processing outbox message {MessageId}",
                message.Id);

            message.RetryCount++;
            message.LastError = ex.Message;
            message.LastAttemptAt = DateTime.UtcNow;

            await repo.Complete();
        }
    }
}