using HiTechStore.Core.Models;
using HiTechStore.Core.Services.Permission;

namespace HiTechStore.Core.Services.Product;

public class ProductPermissionHelper(IPermissionService permissionService)
{
    public Task<bool> HasProductCreatePermission(string userId)
    {
        return permissionService.HasPermissions(
            userId,
            [
                Permissions.Product.Create
            ]
        );
    }

    public Task<bool> HasProductDeletePermission(string userId)
    {
        return permissionService.HasPermissions(
            userId,
            [
                Permissions.Product.Delete
            ]
        );
    }

    public Task<bool> HasProductEditPermission(string userId)
    {
        return permissionService.HasPermissions(
            userId,
            [
                Permissions.Product.Edit
            ]
        );
    }
}