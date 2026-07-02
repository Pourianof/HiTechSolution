using System.ComponentModel.DataAnnotations;

using HiTechStore.Core.Dto.Permission;
using HiTechStore.Helpers.AutoMapper;

namespace HiTechStore.Presentation.Requests.Auth;

[MapTo<ModifyPermissionDto>]
public class UpdatePermissionsRequest
{
    [Required]
    [MinLength(1)]
    public IEnumerable<PermissionChangeRequest>? Permissions { get; set; }
}

[MapTo<TargetPermissionDto>]
public class PermissionChangeRequest
{
    [Required]
    public string? PermissionCode { get; set; }
    [Required]
    public string? Action { get; set; }
    [Required]
    public string? Scope { get; set; }
}