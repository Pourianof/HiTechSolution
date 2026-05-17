using HiTechStore.Core.Common.Interfaces.Infra;
using HiTechStore.Data.Storage;
using HiTechStore.Infrastructure.ThumbnailGenerator;

namespace HiTechStore.Infrastructure;

public static class IngrastructureDependencyRegistration
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddTransient<IThumbnailGenerator, FfmpegProcessThumbnailGenerator>();
        services.AddTransient<IPublicAssetRegisterer, LocalWWWRootAssetRegisterer>();
        services.AddTransient<ProductMediaRegisterer>();

        return services;
    }
}