using HiTechStore.ApiTokenHandler;
using HiTechStore.Core.Common.Interfaces.Presentation;
using HiTechStore.Helpers.Types;
using HiTechStore.Presentation.Auth;

namespace HiTechStore.Presentation;

public static class DependencyRegistration
{
    public static WebApplicationBuilder UsePresentation(this WebApplicationBuilder builder)
    {

        builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(options =>
        {
            if (int.TryParse(builder.Configuration["FormLimit"], out var limit))
            {
                options.MultipartBodyLengthLimit = limit;
            }

        });

        builder.Services.AddScoped<ICurrentUserProvider>(
            sp =>
            {
                var httpContext = sp.GetRequiredService<IHttpContextAccessor>().HttpContext;
                return new CurrentUserProvider(httpContext?.User);
            }
        );

        builder.Services.AddTokenHandler(builder.Configuration.ProvideConnectionString());
        builder.Services.AddSingleton<IApplicationContext, ApplicationContext>();
        builder.Services.WithRateLimiter();

        return builder;
    }
}