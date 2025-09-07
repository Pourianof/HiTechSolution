using HiTechStore.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace HiTechStore.Data.Seeders
{
    public static class AdminSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetService<UserManager<User>>();

            var adminEmail = serviceProvider.GetRequiredService<IConfiguration>()["AdminEmail"];
            var adminPassword = serviceProvider.GetRequiredService<IConfiguration>()["AdminPassword"];

            if (string.IsNullOrEmpty(adminEmail))
            {
                throw new Exception("Admin email is not configured.");
            }

            if (string.IsNullOrEmpty(adminPassword))
            {
                throw new Exception("Admin password is not configured.");
            }



            var adminUser = await userManager!.FindByEmailAsync(adminEmail!);

            if (adminUser == null)
            {
                adminUser = new User
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var adminCreateResult = await userManager.CreateAsync(adminUser, adminPassword!);
                if (!adminCreateResult.Succeeded)
                {
                    throw new Exception("Failed to create admin user." + string.Join(", ", adminCreateResult.Errors.Select(e => e.Description)));
                }

                var addToRoleResult = await userManager.AddToRoleAsync(adminUser, IdentityRoles.Admin);
                if (!addToRoleResult.Succeeded)
                {
                    await userManager.DeleteAsync(adminUser);
                    throw new Exception("Failed to add admin user to role." + string.Join(", ", addToRoleResult.Errors.Select(e => e.Description)));
                }
            }
        }
    }
}