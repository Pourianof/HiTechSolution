using HiTechStore.Core;
using HiTechStore.Core.Models;

using Microsoft.AspNetCore.Identity;

namespace HiTechStore.Infrastructure.Data.Seeders;

public static class SeederExtension
{
    public static async Task SeedDatabase(this WebApplication app)
    {
        using var service = app.Services.CreateScope();
        var uow = service.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var userManager = service.ServiceProvider.GetRequiredService<UserManager<User>>();
        var configs = service.ServiceProvider.GetRequiredService<IConfiguration>();

        await RoleSeeder.SeedAsync(service.ServiceProvider);
        await AdminSeeder.SeedAsync(userManager, configs);
        await ColorSeeder.SeedAsync(uow);
        await uow.SeedDiscountEntitiesAsync();
        await uow.SeedConditionMethodssAsync();

        if (app.Environment.IsDevelopment())
        {
            await UserSeeder.SeedAsync(userManager);
            await BrandSeeder.SeedAsync(uow);
            await ComponentSeeder.SeedAsync(uow);
            await CategorySeeder.SeedAsync(uow);
            await ProductsSeeder.SeedAsync(uow, userManager);
        }
    }
}