namespace HiTechStore.Infrastructure.Dispatcher;

public interface IOutboxDispatcherRegistry
{
    IReadOnlyList<IOutboxDispatcher> GetDispatchers(Type eventType);
}

public class OutboxDispatcherRegistry
    : IOutboxDispatcherRegistry
{
    private readonly Dictionary<Type, List<IOutboxDispatcher>> _map;

    public OutboxDispatcherRegistry(IServiceScopeFactory serviceScopeFactory)
    {
        using var scope = serviceScopeFactory.CreateScope();

        var dispatchers = scope.ServiceProvider.GetServices<IOutboxDispatcher>();

        _map = new Dictionary<Type, List<IOutboxDispatcher>>();

        foreach (var dispatcher in dispatchers)
        {
            var dispatcherType = dispatcher.GetType();
            var genericInterface = dispatcherType.GetInterfaces()
                .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IOutboxDispatcher<>));

            if (genericInterface != null)
            {
                var eventType = genericInterface.GetGenericArguments()[0];

                if (!_map.ContainsKey(eventType))
                {
                    _map[eventType] = new List<IOutboxDispatcher>();
                }

                _map[eventType].Add(dispatcher);
            }
        }
    }

    public IReadOnlyList<IOutboxDispatcher> GetDispatchers(Type eventType)
    {
        return _map.TryGetValue(eventType, out var handlers)
            ? handlers
            : [];
    }
}