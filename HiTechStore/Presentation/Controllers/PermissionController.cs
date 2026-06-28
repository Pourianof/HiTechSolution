
using AutoMapper;

using HiTechStore.Core.Dto.Permission;
using HiTechStore.Core.Services.Permission;
using HiTechStore.Presentation.Requests.Auth;
using HiTechStore.Presentation.Responses.Permission;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HiTechStore.Presentation.Controllers;

[Route("/api/auth")]
[Authorize]
public class PermissionController(
    IPermissionService permissionService,
    IMapper mapper
    ) : AppControllerBase
{
    [HttpPatch("{userId:guid}/permissions")]
    public async Task<ActionResult<UpdatePermissionResponse>> UpdateUserPermissions(string userId, UpdatePermissionsRequest request)
    {
        var modifyDto = mapper.Map<ModifyPermissionDto>(request);
        modifyDto.TargetUserId = userId;

        var result = await permissionService.ModifyPermissions(modifyDto);

        return ResultCheck(
            result,
            "Update permission failed"
        );
    }
}