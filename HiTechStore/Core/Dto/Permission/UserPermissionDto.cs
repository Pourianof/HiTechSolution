using HiTechStore.Core.Models;
using HiTechStore.Helpers.AutoMapper;

namespace HiTechStore.Core.Dto.Permission;

[MapFrom<UserPermission>]
public class UserPermissionDto
{
    [MapFromProperty(path: [nameof(UserPermission.Permission), nameof(UserPermission.Permission.Code)])]
    required public string Code { get; set; }
    public PermissionScope? Scope { get; set; } = default;
}