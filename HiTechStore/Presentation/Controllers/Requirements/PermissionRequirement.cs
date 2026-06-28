using Microsoft.AspNetCore.Authorization;

namespace HiTechStore.Presentation.Controllers.Requirements;


public class PermissionRequirement(string permissionCode) : IAuthorizationRequirement
{
    public string PermissionCode { get; set; } = permissionCode;
}