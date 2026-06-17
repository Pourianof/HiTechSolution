using HiTechStore.Core.Common.Interfaces.Infra;
using HiTechStore.Infrastructure.AssetStorage;
using HiTechStore.Infrastructure.Email;
using HiTechStore.Infrastructure.ThumbnailGenerator;
using HiTechStore.Infrastructure.Utils;

namespace HiTechStore.Infrastructure;

public static class IngrastructureDependencyRegistration
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IThumbnailGenerator, FfmpegProcessThumbnailGenerator>();
        services.AddTransient<IPublicAssetRegisterer, LocalWWWRootAssetRegisterer>();
        services.AddTransient<ProductMediaRegisterer>();
        services.AddTransient<IWellDistributedPathGenerator, Sha256TwoPartDistributedPathGenerator>();

        services.AddOptions<EmailSettings>().BindConfiguration("Email");
        services.AddScoped<IEmailSender, MailKitEmailSender>();
        services.AddSingleton<IEmailTemplateRenderer, FileEmailTemplateRenderer>();
        services.AddScoped<IEmailNotificationService, EmailNotificationService>();

        services.UseStorage(configuration);

        return services;
    }
}