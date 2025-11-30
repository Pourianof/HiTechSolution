using HiTechPay.Sdk;
using HiTechPay.Sdk.Keys;

using Microsoft.Extensions.DependencyInjection;

public static class ServiceRegistration
{
    public static IServiceCollection UseHiTechPaySdk(this IServiceCollection services)
    {
        services.AddScoped<IVerifier, Verifier>();
        services.AddScoped<IHiTechPaySdkFacade, HiTechPaySdkFacade>();

        return services;
    }
}