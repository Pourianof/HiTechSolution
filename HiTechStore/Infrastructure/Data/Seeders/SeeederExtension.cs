using HiTechStore.Core;
using HiTechStore.Core.Models;

using Microsoft.AspNetCore.Identity;

namespace HiTechStore.Infrastructure.Data.Seeders;

public static class SeederExtension
{
    public static async Task SeedDatabase(this WebApplication app)
    {

        using var scope = app.Services.CreateScope();
        var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

        await SeedRequiredBaseData(scope.ServiceProvider);

        if (app.Environment.IsDevelopment())
        {
            await UserSeeder.SeedAsync(userManager);
            await BrandSeeder.SeedAsync(uow);
            await ComponentSeeder.SeedAsync(uow);
            await CategorySeeder.SeedAsync(uow);
            await ProductsSeeder.SeedAsync(uow, userManager);
        }
    }

    public static async Task SeedRequiredBaseData(IServiceProvider serviceProvider)
    {
        var uow = serviceProvider.GetRequiredService<IUnitOfWork>();
        var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
        var configs = serviceProvider.GetRequiredService<IConfiguration>();

        await uow.SeedPermissions();
        await RoleSeeder.SeedAsync(serviceProvider);
        await AdminSeeder.SeedAsync(userManager, configs);
        await uow.SeedAminPermissions();
        await ColorSeeder.SeedAsync(uow);
        await uow.SeedDiscountEntitiesAsync();
        await uow.SeedConditionMethodssAsync();
    }
}