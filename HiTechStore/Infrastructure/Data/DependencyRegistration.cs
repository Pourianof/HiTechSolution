using HiTechStore.Core;
using HiTechStore.Core.Common.Interfaces.Infra;
using HiTechStore.Infrastructure.Data.Repositories;
using HiTechStore.Infrastructure.Data.Storage;

using Microsoft.EntityFrameworkCore;

namespace HiTechStore.Infrastructure.Data;

public static class DataDependencyRegistration
{
    public static IHostApplicationBuilder UseDataAccess(this IHostApplicationBuilder builder)
    {
        var baseConnStr = builder.Configuration.GetConnectionString("DefaultConnection");
        var username = builder.Configuration["Db:Username"];
        var password = builder.Configuration["Db:Password"];

        var fullConnStr = $"{baseConnStr}Username={username};Password={password}";

        builder.Services.AddDbContext<HiTechStoreDbContext>(options =>
            {
                options.UseNpgsql(fullConnStr);
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