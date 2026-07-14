namespace HiTechStore.Infrastructure.Dispatcher;

public static class DispatcherDependencyRegistration
{
    public static void UseDispatcher(this IServiceCollection services)
    {
        services.AddScoped<IOutboxDispatcher, UserNotificationCreatedDispatcher>();
        services.AddScoped<IOutboxDispatcher, PermissionChangeUserNotificationHandler>();

        services.AddSingleton<EventTypeResolver>();

    }
}