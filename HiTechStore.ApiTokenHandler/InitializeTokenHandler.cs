using HiTechStore.ApiTokenHandler.Infrastructure;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HiTechStore.ApiTokenHandler;

public static class TokenHandlerInitializer
{
    public static async Task Initialize(TokenHandlerServiceContext context)
    {
        if (context.Environment.IsProduction())
        {
            using var db = context.ServiceProvider.GetRequiredService<AuthTokensDbContext>();

            await db.Database.MigrateAsync();
        }
    }
}

public class TokenHandlerServiceContext
{
    required public IServiceProvider ServiceProvider { get; set; }
    public AppEnvironment Environment { get; set; }
}

public enum AppEnvironment
{
    Development,
    Production
}


public static class AppEnvironmentExtension
{
    public static bool IsProduction(this AppEnvironment env)
    {
        return env == AppEnvironment.Production;
    }
}