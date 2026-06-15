using HiTechStore.Core;
using HiTechStore.Core.Common.Interfaces.Infra;
using HiTechStore.Helpers.Types;
using HiTechStore.Infrastructure.AssetStorage;
using HiTechStore.Infrastructure.Data.Repositories;
using HiTechStore.Infrastructure.Data.Storage;

using Microsoft.EntityFrameworkCore;

using Npgsql;

namespace HiTechStore.Infrastructure.Data;

public static class DataDependencyRegistration
{
    public static IHostApplicationBuilder UseDataAccess(this IHostApplicationBuilder builder)
    {
        builder.Services.AddDbContext<HiTechStoreDbContext>(options =>
            {
                options.UseNpgsql(builder.Configuration.ProviderConnectionString());
                if (builder.Environment.IsDevelopment())
                {
                    options.EnableSensitiveDataLogging();
                }
            }
        );

        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
        builder.Services.AddRepositories();

        builder.Services.AddTransient<IPublicAssetRegisterer, LocalWWWRootAssetRegisterer>();
        builder.Services.AddTransient<ICategoryAssetHelper, CategoryAssetHelper>();

        return builder;
    }
}