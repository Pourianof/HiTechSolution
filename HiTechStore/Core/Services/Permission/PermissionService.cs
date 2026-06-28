
using HiTechStore.Core.Common.Interfaces.Presentation;
using HiTechStore.Core.Dto.Auth;
using HiTechStore.Core.Dto.Permission;
using HiTechStore.Core.Exceptions;
using HiTechStore.Core.Helpers.Models;
using HiTechStore.Core.Helpers.Result;
using HiTechStore.Core.Models;
using HiTechStore.Core.Services.Authorization;

namespace HiTechStore.Core.Services.Permission;

public class PermissionService : ServiceBase, IPermissionService
{
    private IUnitOfWork _unitOfWork;
    public PermissionService(
        IUnitOfWork unitOfWork,
        IAuthorizationService authorizationService,
        ICurrentUserProvider currentUserProvider
    ) : base(authorizationService, currentUserProvider)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> HasPermissions(string userId, IEnumerable<string> permissionCodes)
    {
        var permissions = await _unitOfWork.PermissionRepository.GetUserPermissions(userId);

        return permissionCodes.All(
            perm => permissions.Any(
                p => perm == p.Code
            )
        );
    }

    public async Task<Result<IEnumerable<Models.Permission>>> ModifyPermissions(ModifyPermissionDto modifyPermissionDto)
    {
        var requestedPermissions = modifyPermissionDto.Permissions.ToList();
        var result = new Result<IEnumerable<Models.Permission>>();

        var actorUser = await GetUser();
        var targetUser = await AuthorizationService.GetUserByIdAsync(modifyPermissionDto.TargetUserId);

        if (targetUser is null)
        {
            return result.AddError(
                PermissionErrors.UserNotFound()
            );
        }

        var actorPermissions = await _unitOfWork.PermissionRepository.GetUserPermissions(actorUser.Id);
        var isActorAdmin = actorUser.IsAdmin();

        if (!isActorAdmin && (targetUser.IsManager() || targetUser.IsAdmin()))
        {
            return result.AddError(
                PermissionErrors.NotAuthorizedToModifyTargetUsersPermissions()
            );
        }
        else if (isActorAdmin && targetUser.IsAdmin())
        {
            return result.AddError(
                PermissionErrors.AdminModifyAdminRestriction()
            );
        }

        if (!isActorAdmin && !actorPermissions.Any(perm => perm.Code == Permissions.Access.Grant))
        {
            return result.AddError(PermissionErrors.GrantPermissionRequiredGrantAccess());
        }

        var permissionCodes = requestedPermissions.Select(p => p.PermissionCode);
        var modifyingPermissions = await _unitOfWork.PermissionRepository.GetPermissionsByCode(permissionCodes);

        // check the targeting permission all are existed
        if (modifyingPermissions.Count() != requestedPermissions.Count())
        {
            var modifyingPermCodes = modifyingPermissions.Select(p => p.Code);

            for (var index = 0; index < requestedPermissions.Count(); index++)
            {
                var perm = requestedPermissions.ElementAt(index);

                if (!modifyingPermCodes.Contains(perm.PermissionCode))
                {
                    result.AddError(
                        PermissionErrors.InvalidPermission(index, perm.PermissionCode)
                    );
                }
            }

            if (!result.IsValid)
            {
                return result;
            }
        }

        var targetUserPermissions = await _unitOfWork.PermissionRepository.GetUserPermissions(targetUser.Id);

        var requestedActions =
            requestedPermissions.ToDictionary(
                x => x.PermissionCode,
                x => x.Action);

        foreach (var permission in modifyingPermissions)
        {
            if (!isActorAdmin && permission.Code == Permissions.Access.Grant)
            {
                return result.AddError(
                    PermissionErrors.ForbiddenAccessGranting()
                );
            }

            if (!actorPermissions.Any((p) => p.Code == permission.Code))
            {
                return result.AddError(
                    PermissionErrors.CannotGrantPermissionYouDoNotHave(permission.Code)
                );
            }

            var action = requestedActions[permission.Code!];
            var didUserHavePermission = targetUserPermissions
                .Any(perm => perm.Code == permission.Code);

            if (action == PermissionModificationAction.Grant)
            {
                // if permission not assigned to user before (duplicate pervent)
                if (!didUserHavePermission)
                {
                    var perm = new UserPermission()
                    {
                        Permission = permission,
                        GrantedByUserId = actorUser.Id
                    };

                    targetUser.Permissions.Add(perm);

                    await _unitOfWork.PermissionAuditRepository.AddAsync(
                        new()
                        {
                            Action = PermissionAction.Granted,
                            ActorUser = actorUser,
                            Permission = permission,
                            TargetUser = targetUser

                        }
                    );
                }
            }
            else if (didUserHavePermission)
            {
                var existingPermission =
                    targetUser.Permissions
                        .FirstOrDefault(
                            p => p.PermissionId == permission.Id);

                if (existingPermission != null)
                {
                    targetUser.Permissions.Remove(existingPermission);
                    await _unitOfWork.PermissionAuditRepository.AddAsync(
                        new()
                        {
                            Action = PermissionAction.Revoked,
                            ActorUser = actorUser,
                            Permission = permission,
                            TargetUser = targetUser

                        }
                    );
                }
            }
        }

        await _unitOfWork.Complete();

        result.Value = await _unitOfWork.PermissionRepository.GetUserPermissions(targetUser.Id);
        return result;
    }
}