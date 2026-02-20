using HiTechStore.Core.Helpers;

namespace HiTechStore.Core.Services;

public static class ServicesDependencyRegistration
{
    static public IServiceCollection AddAppServices(this IServiceCollection services)
    {
        services.AddScoped<IDiscountCodeGenerator, DiscountCodeGenerator>();
        services.AddScoped<IDiscountService, DiscountService>();

        return services;
    }
}
