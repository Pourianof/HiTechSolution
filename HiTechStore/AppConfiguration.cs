using HiTechStore.ApiTokenHandler;
using HiTechStore.Infrastructure.Data;
using HiTechStore.Presentation.Auth;
using HiTechStore.Presentation.RealTime;

namespace HiTechStore;

public static class AppConfiguration
{
    public static async Task ConfigueApp(this WebApplication app)
    {
        app.UseExceptionHandler();

        await app.ConfigueAuth();

        app.UseStaticFiles(
            new StaticFileOptions
            {
                OnPrepareResponse = (context) =>
                {
                    var headers = context.Context.Response.GetTypedHeaders();
                    headers.CacheControl = new Microsoft.Net.Http.Headers.CacheControlHeaderValue
                    {
                        // Need to optimize for some really static assets
                        NoCache = true,
                        MustRevalidate = true,
                        MaxAge = TimeSpan.Zero,
                        NoStore = true
                    };
                }
            }
        );
        app.UseRateLimiter();
        app.MapControllers();

        app.UseHealthChecks("/_health");

        app.MapHub<NotificationHub>(NotificationHub.Route);

        await app.DbInitialize();

        using (var scope = app.Services.CreateScope())
        {
            await TokenHandlerInitializer.Initialize(new()
            {
                ServiceProvider = scope.ServiceProvider,
                Environment = app.Environment.IsProduction() ? AppEnvironment.Production : AppEnvironment.Development
            });
        }

    }
}