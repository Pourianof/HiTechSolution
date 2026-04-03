using HiTechStore.Core.Helpers;
using HiTechStore.Core.Services.Discount;

namespace HiTechStore.Core.Services;

public static class ServicesDependencyRegistration
{
    static public IServiceCollection AddAppServices(this IServiceCollection services)
    {
        services.AddScoped<IDiscountCodeGenerator, DiscountCodeGenerator>();
        services.AddScoped<IDiscountService, DiscountService>();
        services.AddScoped<IDiscountConditionValueComaprator, DiscountConditionValueComaprator>();

        return services;
    }
}
