using HiTechStore.Core.Auth;
using HiTechStore.Presentation.Auth;

namespace HiTechStore.Presentation;

public static class DependencyRegistration
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddScoped<ICurrentUserProvider>(
            sp =>
            {
                var httpContext = sp.GetRequiredService<IHttpContextAccessor>().HttpContext;
                return new CurrentUserProvider(httpContext?.User);
            }
        );

        return services;
    }
}