using HiTechStore.Core.Models;

using Microsoft.AspNetCore.Identity;

namespace HiTechStore.Presentation.Auth;

public static class AuthConfiguration
{
    public static async Task ConfigueAuth(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            foreach (var role in IdentityRoles.AllRoles)
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
        }

        app.UseAuthentication();
        app.UseAuthorization();
    }
}