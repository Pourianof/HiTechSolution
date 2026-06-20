using HiTechPay.Sdk;
using HiTechPay.Sdk.Communication;
using HiTechPay.Sdk.Keys;

using Microsoft.Extensions.DependencyInjection;

public static class ServiceRegistration
{
    public static IServiceCollection UseHiTechPaySdk(this IServiceCollection services, string? paymentServerUrl)
    {
        services.AddTransient<ServerConnectionContext>(
            (provider) => new() { PaymentServerAddress = paymentServerUrl }
        );
        services.AddScoped<IVerifier, Verifier>();
        services.AddScoped<IServerConnectionHelper, ServerConnectionHelper>();
        services.AddScoped<IHiTechPaySdkFacade, HiTechPaySdkFacade>();

        return services;
    }
}