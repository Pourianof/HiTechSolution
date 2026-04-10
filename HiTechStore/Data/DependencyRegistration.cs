using HiTechStore.Core;
using HiTechStore.Data.Repositories;
using HiTechStore.Data.Storage;

using Microsoft.EntityFrameworkCore;

namespace HiTechStore.Data;

public static class DataDependencyRegistration
{
    public static IHostApplicationBuilder UseDataAccess(this IHostApplicationBuilder builder)
    {
        var baseConnStr = builder.Configuration.GetConnectionString("DefaultConnection");
        var username = builder.Configuration["Db:Username"];
        var password = builder.Configuration["Db:Password"];

        var fullConnStr = $"{baseConnStr}Username={username};Password={password}";

        builder.Services.AddDbContext<HiTechStoreDbContext>(options =>
            options.UseNpgsql(fullConnStr)
            );

        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
        builder.Services.AddRepositories();

        builder.Services.AddTransient<IPublicAssetRegisterer, LocalWWWRootAssetRegisterer>();
        builder.Services.AddTransient<ICategoryAssetHelper, CategoryAssetHelper>();

        return builder;
    }
}