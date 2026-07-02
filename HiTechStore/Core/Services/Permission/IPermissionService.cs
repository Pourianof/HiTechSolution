using HiTechStore.Core.Dto.Permission;
using HiTechStore.Core.Helpers.Result;

namespace HiTechStore.Core.Services.Permission;

public interface IPermissionService
{
    Task<Result<IEnumerable<UserPermissionDto>>> ModifyPermissions(ModifyPermissionDto modifyPermissionDto);
    Task<bool> HasAllPermissions(string userId, IEnumerable<UserPermissionDto> permissionCodes);
    Task<bool> HasAnyPermissions(string userId, IEnumerable<UserPermissionDto> permissionCodes);
    Task<bool> HasResourceAccess(string userId, IEnumerable<ResourceAccessCheck> permissionCodes);
}

public class ResourceAccessCheck
{
    required public bool IsOwner { get; set; }
    required public string Code { get; set; }
}