using System;

using HiTechStore.ApiTokenHandler.Core;
using HiTechStore.ApiTokenHandler.Infrastructure;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HiTechStore.ApiTokenHandler;

public static class DependencyRegistrations
{
    public static IServiceCollection AddTokenHandler(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<AuthTokensDbContext>(options =>
            {
                options.UseNpgsql(connectionString);
            }
        );

        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

        services.AddTransient<IRandomSecureTokenGenerator, RandomTokenGenerator>();
        services.AddScoped<ITokenRepository, EfTokenRepository>();
        services.AddScoped<ITokenHandler, JwtTokenHandler>();

        return services;
    }
}
