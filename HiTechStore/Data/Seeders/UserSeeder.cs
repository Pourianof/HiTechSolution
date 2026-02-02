using HiTechStore.Models;

using Microsoft.AspNetCore.Identity;

namespace HiTechStore.Data.Seeders
{
    public static class UserSeeder
    {
        public static async Task SeedAsync(UserManager<User> userManager)
        {
            var user = new User
            {
                Email = "manager@gmail.com",
                FirstName = "Kazem",
                LastName = "Ghaghi",
                UserName = "manager",
                EmailConfirmed = true
            };

            await userManager.CreateAsync(
               user, "Test123!"
            );

            await userManager.AddToRoleAsync(user, IdentityRoles.Manager);
        }
    }
}