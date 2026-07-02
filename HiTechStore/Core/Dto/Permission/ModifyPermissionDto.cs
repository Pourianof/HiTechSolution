using HiTechStore.Core.Models;

namespace HiTechStore.Core.Dto.Permission;

public class ModifyPermissionDto
{
    required public IEnumerable<TargetPermissionDto> Permissions { get; set; }
    required public string TargetUserId { get; set; }
}

public class TargetPermissionDto
{
    public PermissionModificationAction Action { get; set; }
    required public string PermissionCode { get; set; }
    required public PermissionScope Scope { get; set; }
}

public enum PermissionModificationAction
{
    Grant,
    Revoke
}