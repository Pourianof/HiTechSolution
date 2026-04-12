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
                foreach (var claim in httpContext!.User.Claims)
                {
                    Console.WriteLine($"{claim.Type} = {claim.Value}");
                }
                return new CurrentUserProvider(httpContext?.User);
            }
        );

        return services;
    }
}