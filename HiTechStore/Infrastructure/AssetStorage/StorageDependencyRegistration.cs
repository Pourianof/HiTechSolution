using HiTechStore.Core.Common.Interfaces.Infra;
using HiTechStore.Infrastructure.Data.Storage;

namespace HiTechStore.Infrastructure.AssetStorage;

public static class StorageDependencyRegistration
{
    public static IServiceCollection UseStorage(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddTransient<ICategoryAssetHelper, CategoryAssetHelper>();

        if (Enum.TryParse<StorageStrategy>(
          configuration["StorageStrategy"],
          ignoreCase: true,
          out var strategy))
        {
            switch (strategy)
            {
                case StorageStrategy.Supabase:
                    services.AddSupabaseStorage(configuration);
                    break;

                case StorageStrategy.Local:
                    services.AddTransient<
                        IPublicAssetRegisterer,
                        LocalWWWRootAssetRegisterer>();
                    break;
                default:
                    throw new InvalidOperationException(
                $"Unknown storage strategy: {configuration["StrategyType"]}");
            }
        }
        else
        {
            throw new InvalidOperationException(
                $"No valid StorageStrategy configuration specified. One of: {string.Join(", ", Enum.GetNames(typeof(StorageStrategy)))}"
            );
        }

        return services;
    }
}