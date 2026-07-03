using HiTechStore.Core.Models;
using HiTechStore.Core.Services.Permission;

namespace HiTechStore.Core.Services.UserService;

public class UsersServicePermissionHelper(IPermissionService permissionService)
{
    public Task<bool> HasPermissionToGetUsersList(string userId)
    {
        return permissionService.HasAllPermissions(
            userId, [
                new (){
                    Code = Permissions.Access.Grant,
                }
            ]
        );
    }
}