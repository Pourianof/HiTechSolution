

using HiTechStore.Core;
using HiTechStore.Data;
using HiTechStore.Data.Seeders;
using HiTechStore.Models;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

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


// Set JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

// Set Identity
builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<HiTechStoreDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();
app.MapControllers();


app.Run();