using HiTechPay.Infrastructure;
using HiTechPay.Services;

namespace HiTechPay;

public static class DependencyInjection
{
    public static IServiceCollection AddDependencies(this IServiceCollection services)
    {
        services.UseSigner();
        services.AddSingleton<IRsaProvider, RsaProvider>();

        return services;
    }
}