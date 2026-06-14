using HiTechStore.ApiTokenHandler;
using HiTechStore.Core.Common.Interfaces.Presentation;
using HiTechStore.Presentation.Auth;

namespace HiTechStore.Presentation;

public static class DependencyRegistration
{
    public static WebApplicationBuilder UsePresentation(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<ICurrentUserProvider>(
            sp =>
            {
                var httpContext = sp.GetRequiredService<IHttpContextAccessor>().HttpContext;
                return new CurrentUserProvider(httpContext?.User);
            }
        );

        var baseConnStr = builder.Configuration.GetConnectionString("DefaultConnection");
        var username = builder.Configuration["Db:Username"];
        var password = builder.Configuration["Db:Password"];

        var fullConnStr = $"{baseConnStr}Username={username};Password={password}";

        builder.Services.AddTokenHandler(fullConnStr);
        builder.Services.AddSingleton<IApplicationContext, ApplicationContext>();
        builder.Services.WithRateLimiter();

        return builder;
    }
}