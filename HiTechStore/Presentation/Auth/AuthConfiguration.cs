using HiTechStore.Core.Models;

using Microsoft.AspNetCore.Identity;

namespace HiTechStore.Presentation.Auth;

public static class AuthConfiguration
{
    public static async Task ConfigueAuth(this WebApplication app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
    }
}