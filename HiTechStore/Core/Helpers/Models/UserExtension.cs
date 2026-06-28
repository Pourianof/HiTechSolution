using HiTechStore.Core.Models;

namespace HiTechStore.Core.Helpers.Models;

public static class UserExtension
{
    public static bool IsAdmin(this User user)
    {
        return user.Roles.Contains(IdentityRoles.Admin);
    }

    public static bool IsManager(this User user)
    {
        return user.Roles.Contains(IdentityRoles.Manager);
    }
}