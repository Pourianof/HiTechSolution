

using HiTechStore.Core;
using HiTechStore.Data;
using HiTechStore.Data.Seeders;

using Microsoft.EntityFrameworkCore;

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

var app = builder.Build();

app.UseStaticFiles();
app.MapControllers();


app.Run();