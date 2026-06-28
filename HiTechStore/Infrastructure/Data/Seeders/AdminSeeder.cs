using HiTechStore.Core.Models;

using Microsoft.AspNetCore.Identity;

namespace HiTechStore.Infrastructure.Data.Seeders
{
    public static class AdminSeeder
    {
        public static async Task SeedAsync(UserManager<User> userManager, IConfiguration configuration)
        {

            var adminEmail = configuration["AdminEmail"];
            var adminPassword = configuration["AdminPassword"];

            if (string.IsNullOrEmpty(adminEmail))
            {
                throw new Exception("Admin email is not configured.");
            }

            if (string.IsNullOrEmpty(adminPassword))
            {
                throw new Exception("Admin password is not configured.");
            }



            var adminUser = await userManager!.FindByEmailAsync(adminEmail!);

            if (adminUser is not null)
            {
                await userManager.RemovePasswordAsync(adminUser);
                await userManager.AddPasswordAsync(adminUser, "Test123!");
                return;
            }

            adminUser = new User
            {
                UserName = "admin",
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