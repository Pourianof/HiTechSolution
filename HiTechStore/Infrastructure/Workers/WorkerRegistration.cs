namespace HiTechStore.Infrastructure.Workers;

public static class WorkerRegistration
{
    public static void AddWorkers(this IServiceCollection services)
    {
        services.AddHostedService<OutboxWorker>();
    }
}