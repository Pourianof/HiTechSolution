using System;

namespace HiTechStore.Core.BackgroundJobs;

public static class BackgroundJobsRegistration
{
    public static IServiceCollection AddBackroundJobs(this IServiceCollection services)
    {
        services.AddHostedService<FailedOrdersRollbackHandler>();

        return services;
    }
}
