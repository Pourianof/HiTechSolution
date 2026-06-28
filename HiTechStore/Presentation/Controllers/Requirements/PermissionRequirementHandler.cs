using System.Security.Claims;

using HiTechStore.Core.Services.Permission;

using Microsoft.AspNetCore.Authorization;

namespace HiTechStore.Presentation.Controllers.Requirements;

public class PermissionRequirementHandler(IPermissionService productPermissionHelper) : AuthorizationHandler<PermissionRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId is null)
            return;

        var hasPermission = await productPermissionHelper.HasPermissions(
            userId,
            [requirement.PermissionCode]);

        if (hasPermission)
        {
            context.Succeed(requirement);
        }
    }
}