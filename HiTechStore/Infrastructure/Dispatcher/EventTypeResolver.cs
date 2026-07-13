using System.Reflection;

using HiTechStore.Core.Common.Events;

namespace HiTechStore.Infrastructure.Dispatcher;

public class EventTypeResolver
{
    public IReadOnlyDictionary<string, Type> Events { get; }
    public EventTypeResolver()
    {
        var eventInterfaceType = typeof(IEvent);

        Events = Assembly
            .GetAssembly(typeof(IEvent))!
            .GetTypes()
            .Where(t => typeof(IEvent).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
            .ToDictionary(t => t.Name);
    }
    public Type? Resolve(string eventName)
    {
        return Events.TryGetValue(eventName, out var type)
            ? type
            : null;
    }
}