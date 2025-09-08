using Core.Authorization;

using HiTechStore.Core;
using HiTechStore.Data;
using HiTechStore.Data.Seeders;
using HiTechStore.Models;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
        .UseSeeding((context, _) =>
        {
            ProductsSeeder.SeedAsync((HiTechStoreDbContext)context).Wait();
        })
        .UseAsyncSeeding(async (context, _, _) =>
        {
            await ProductsSeeder.SeedAsync((HiTechStoreDbContext)context);
        })
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

builder.Services.AddScoped<IAuthorizationHandler, SameAuthorAccessAuthorization>();
builder.Services.AddScoped<IAuthorizationHandler, AdminIsAlwaysAuthorizedAuthorization>();

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

app.UseStaticFiles();
app.MapControllers();


app.Run();