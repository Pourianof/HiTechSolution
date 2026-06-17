using HiTechStore.Core;
using HiTechStore.Helpers.Types;
using HiTechStore.Infrastructure.Data.Repositories;

using Microsoft.EntityFrameworkCore;

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

        return builder;
    }
}