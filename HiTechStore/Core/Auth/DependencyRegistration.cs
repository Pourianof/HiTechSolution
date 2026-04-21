using System.Security.Claims;
using System.Text;

using HiTechStore.ApiTokenHandler.Core;
using HiTechStore.Core.Auth.Authorization;
using HiTechStore.Data;
using HiTechStore.Models;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace HiTechStore.Core.Auth;


public static class AuthRegistration
{
    public static IHostApplicationBuilder AddAuth(this IHostApplicationBuilder builder)
    {
        // Set Identity
        builder.Services.AddIdentity<User, IdentityRole>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<HiTechStoreDbContext>()
            .AddDefaultTokenProviders();

        // Set JWT
        builder.Services.AddAuthentication(
            (options) =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }
        ).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.Events = new JwtBearerEvents()
                {
                    OnChallenge = async (context) =>
                    {
                        context.HandleResponse();

                        var problem = new ProblemDetails
                        {
                            Status = StatusCodes.Status401Unauthorized,
                            Title = "Unauthorized",
                            Detail = "Authorization failed. Try to login again.",
                            Instance = context.Request.Path
                        };

                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/problem+json";

                        await context.Response.WriteAsJsonAsync(problem);
                    },
                    OnForbidden = async (context) =>
                    {
                        var problem = new ProblemDetails
                        {
                            Status = StatusCodes.Status403Forbidden,
                            Title = "Forbidden",
                            Detail = "You don't have access to this route.",
                            Instance = context.Request.Path
                        };

                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        context.Response.ContentType = "application/problem+json";

                        await context.Response.WriteAsJsonAsync(problem);
                    },
                    OnTokenValidated = async (context) =>
                    {
                        var logger = context.HttpContext.RequestServices
                                            .GetRequiredService<ILogger<Program>>();

                        if (context.Principal?.Claims is null)
                        {
                            logger.LogWarning("user has not any claim");

                            context.Fail("No token claims detected");
                            return;
                        }

                        var serviceProvider = context.HttpContext.RequestServices;

                        var tokenHandler = serviceProvider.GetRequiredService<ITokenHandler>();


                        var isValid = await tokenHandler.IsJwtTokenAuthorized(context.Principal.Claims);

                        if (isValid)
                        {
                            logger.LogInformation("user token was not validate in term of refresh token");

                            context.Fail("User is not valid.");
                            return;
                        }
                    }
                };
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    RoleClaimType = ClaimTypes.Role,
                    NameClaimType = ClaimTypes.Name,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
                };
            });


        builder.Services.AddAuthorization();
        builder.Services.AddProblemDetails();

        builder.Services.AddScoped<IAuthorizationHandler, SameAuthorAccessAuthorization>();
        builder.Services.AddScoped<IAuthorizationHandler, AdminIsAlwaysAuthorizedAuthorization>();

        return builder;
    }
}