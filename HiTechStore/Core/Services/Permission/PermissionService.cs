
using HiTechStore.Core.Common.Events;
using HiTechStore.Core.Common.Interfaces.Infra;
using HiTechStore.Core.Common.Interfaces.Presentation;
using HiTechStore.Core.Dto.Permission;
using HiTechStore.Core.Helpers.Models;
using HiTechStore.Core.Helpers.Result;
using HiTechStore.Core.Models;
using HiTechStore.Core.Services.Authorization;

namespace HiTechStore.Core.Services.Permission;

public class PermissionService : ServiceBase, IPermissionService
{
    private IUnitOfWork _unitOfWork;
    private IEventPublisher _eventPublisher;
    public PermissionService(
        IUnitOfWork unitOfWork,
        IAuthorizationService authorizationService,
        ICurrentUserProvider currentUserProvider,
        IEventPublisher eventPublisher
    ) : base(authorizationService, currentUserProvider)
    {
        _unitOfWork = unitOfWork;
        _eventPublisher = eventPublisher;
    }

    private Task<IEnumerable<UserPermissionDto>> GetUsersPermissions(string userId)
    {
        return _unitOfWork.PermissionRepository.GetUserPermissions(userId);
    }

    public async Task<bool> HasAllPermissions(string userId, IEnumerable<UserPermissionDto> permissionCodes)
    {
        var userPermissions = await GetUsersPermissions(userId);

        return permissionCodes.All(
            perm => userPermissions.Any(
                p => perm.Code == p.Code && (
                    p.Scope is null || // if is null then its free(no-scope)
                    p.Scope == PermissionScope.All ||
                    perm.Scope == p.Scope)
            )
        );
    }

    public async Task<bool> HasAnyPermissions(string userId, IEnumerable<UserPermissionDto> permissionCodes)
    {
        var userPermissions = await GetUsersPermissions(userId);

        return permissionCodes.Any(
            perm => userPermissions.Any(
                p => perm.Code == p.Code && (
                    p.Scope is null || // if is null then its free(no-scope)
                    p.Scope == PermissionScope.All ||
                    perm.Scope == p.Scope)
            )
        );
    }

    public async Task<bool> HasResourceAccess(string userId, IEnumerable<ResourceAccessCheck> permissionCodes)
    {
        var userPermissions = await GetUsersPermissions(userId);

        return permissionCodes.All(
            perm => userPermissions.Any(
                up => perm.Code == up.Code && (
                    up.Scope == PermissionScope.All ||
                    (up.Scope == PermissionScope.Self && perm.IsOwner)
                )
            )
        );
    }

    public async Task<Result<IEnumerable<UserPermissionDto>>> ModifyPermissions(ModifyPermissionDto modifyPermissionDto)
    {
        var requestedPermissions = modifyPermissionDto.Permissions.ToList();
        var result = new Result<IEnumerable<UserPermissionDto>>();

        var actorUser = await GetUser();
        var targetUser = await AuthorizationService.GetUserByIdAsync(modifyPermissionDto.TargetUserId);

        if (targetUser is null)
        {
            return result.AddError(
                PermissionErrors.UserNotFound()
            );
        }

        var actorPermissions = actorUser.Permissions ?? [];
        var isActorAdmin = actorUser.IsAdmin();
        var targetUserHasAccessPermission = targetUser.Permissions?.Any(p => p.Permission!.Code == Permissions.Access.Grant) == true;

        if (!isActorAdmin && !actorUser.IsManager() && targetUserHasAccessPermission)
        {
            return result.AddError(
                PermissionErrors.LockingPermissionListForAccessGrantedTargetUser()
            );
        }

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

        if (!isActorAdmin && !actorPermissions.Any(perm => perm.Permission!.Code == Permissions.Access.Grant))
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
        targetUser.Permissions ??= [];

        var requestedPermMap =
            requestedPermissions.ToDictionary(
                x => x.PermissionCode,
                x => new { x.Action, x.Scope });

        foreach (var permission in modifyingPermissions)
        {
            if (!isActorAdmin && permission.Code == Permissions.Access.Grant)
            {
                return result.AddError(
                    PermissionErrors.ForbiddenAccessGranting()
                );
            }
            var reqPerm = requestedPermMap[permission.Code!];

            bool permCodeMatch = true, scopeMatch = true;
            if (!actorPermissions.Any((p) => (permCodeMatch = p.Permission!.Code == permission.Code) && (scopeMatch = p.Scope == PermissionScope.All || p.Scope == reqPerm.Scope)))
            {
                if (!permCodeMatch)
                {
                    return result.AddError(
                        PermissionErrors.CannotGrantPermissionYouDoNotHave(permission.Code)
                    );
                }
                else
                {
                    // scope mismatch
                    return result.AddError(
                        PermissionErrors.CannotGrantPermissionScopeWhichHigherThanYou(permission.Code, Enum.GetName(reqPerm.Scope) ?? "")
                    );
                }
            }

            var existingPermission = targetUser.Permissions?
               .FirstOrDefault(perm => perm.Permission!.Code == permission.Code);
            var didUserHavePermission = existingPermission is not null;

            var hasChanged = false;

            if (reqPerm.Action == PermissionModificationAction.Grant)
            {
                var isScopeChange = didUserHavePermission && existingPermission!.Scope != reqPerm.Scope;

                if (isScopeChange)
                {
                    targetUser.Permissions!.Remove(existingPermission!);
                }

                // if permission not assigned to user before (duplicate pervent)
                if (!didUserHavePermission || isScopeChange)
                {
                    var perm = new UserPermission()
                    {
                        Permission = permission,
                        GrantedByUserId = actorUser.Id
                    };
                    targetUser.Permissions!.Add(perm);

                    await _unitOfWork.PermissionAuditRepository.AddAsync(
                        new()
                        {
                            Action = PermissionAction.Granted,
                            ActorUser = actorUser,
                            Permission = permission,
                            TargetUser = targetUser,
                            Scope = reqPerm.Scope
                        }
                    );

                    hasChanged = true;
                }
            }
            else if (didUserHavePermission)
            {
                if (existingPermission != null)
                {
                    // not allowed to revoke higher scope permission
                    if (existingPermission.Scope == PermissionScope.Self && reqPerm.Scope == PermissionScope.All)
                    {
                        return result.AddError(
                            PermissionErrors.CannotGrantPermissionScopeWhichHigherThanYou(permission.Code, Enum.GetName(reqPerm.Scope) ?? "")
                        );
                    }

                    targetUser.Permissions!.Remove(existingPermission);
                    await _unitOfWork.PermissionAuditRepository.AddAsync(
                        new()
                        {
                            Action = PermissionAction.Revoked,
                            ActorUser = actorUser,
                            Permission = permission,
                            TargetUser = targetUser
                        }
                    );

                    hasChanged = true;
                }
            }

            if (hasChanged)
            {
                await _eventPublisher.PublishAsync(
                    new PermissionChangedEvent()
                    {
                        TargetUserId = targetUser.Id
                    }
                );
            }
        }

        await _unitOfWork.Complete();

        result.Value = await _unitOfWork.PermissionRepository.GetUserPermissions(targetUser.Id);
        return result;
    }
}