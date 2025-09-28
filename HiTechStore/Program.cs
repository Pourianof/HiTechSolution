using Core.Authorization;

using HiTechStore.Controllers.ExceptionFilters;
using HiTechStore.Core;
using HiTechStore.Core.ExceptionHandlers;
using HiTechStore.Data;
using HiTechStore.Data.Seeders;
using HiTechStore.Models;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();

var baseConnStr = builder.Configuration.GetConnectionString("DefaultConnection");
var username = builder.Configuration["Db:Username"];
var password = builder.Configuration["Db:Password"];

var fullConnStr = $"{baseConnStr}Username={username};Password={password}";

builder.Services.AddDbContext<HiTechStoreDbContext>(options =>
    options.UseNpgsql(fullConnStr)
    );

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddAutoMapper(typeof(MappingProfile));

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

builder.Services.AddExceptionHandler<PgDbExceptionHandler>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    foreach (var role in IdentityRoles.AllRoles)
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));

    await AdminSeeder.SeedAsync(scope.ServiceProvider);
}

app.UseAuthentication();
app.UseAuthorization();

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
app.MapControllers();
app.UseExceptionHandler();


app.Run();