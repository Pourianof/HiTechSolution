namespace HiTechStore.Infrastructure.Dispatcher;

public static class DispatcherDependencyRegistration
{
    public static void UseDispatcher(this IServiceCollection services)
    {
        services.AddScoped<IOutboxDispatcherRegistry, OutboxDispatcherRegistry>();
        services.AddScoped<IOutboxDispatcher, PermissionChangeDispatcher>();
        services.AddSingleton<EventTypeResolver>();

    }
}