using HiTechStore.Core.Models;

namespace HiTechStore.Core.Dto.Permission;

public class UserPermissionDto
{
    required public string Code { get; set; }
    public PermissionScope? Scope { get; set; } = default;
}