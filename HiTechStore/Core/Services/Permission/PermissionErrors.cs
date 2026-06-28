using HiTechStore.Core.Dto.Permission;
using HiTechStore.Core.Helpers.Result;

namespace HiTechStore.Core.Services.Permission;

public static class PermissionErrors
{
    public static ResultError GrantPermissionRequiredGrantAccess() => new()
    {
        Title = "Forbidden operation",
        Description = "Not authorized to grant permission to other",
        Code = nameof(GrantPermissionRequiredGrantAccess)
    };

    public static ValidationResultError UserNotFound() => new()
    {
        Title = "No user found",
        Description = "No user found with specified id",
        Code = nameof(UserNotFound),
        FieldName = "UserId"
    };

    public static ValidationResultError InvalidPermission(int index, string code) => new()
    {
        Title = "Permission is invalid",
        Description = @$"Permission code ""{code}"" not exist",
        Code = nameof(InvalidPermission),
        FieldName = string.Join('.', [nameof(ModifyPermissionDto.Permissions), index.ToString(), nameof(TargetPermissionDto.PermissionCode)])
    };

    public static ResultError CannotGrantPermissionYouDoNotHave(string code) => new()
    {
        Title = "Grant failed",
        Description = $"You cannot grant or revoke a permission({code}) which you don't have",
        Code = nameof(CannotGrantPermissionYouDoNotHave),
    };

    public static ResultError NotAuthorizedToModifyTargetUsersPermissions() => new()
    {
        Title = "Un-authorized",
        Description = "You are not authorized to change or modify target user's permissions",
        Code = nameof(NotAuthorizedToModifyTargetUsersPermissions)
    };
    public static ResultError AdminModifyAdminRestriction() => new()
    {
        Title = "Admin restriction",
        Description = "Admin user try to modify admin's persmissions are not allowed",
        Code = nameof(AdminModifyAdminRestriction)
    };
    public static ResultError ForbiddenAccessGranting() => new()
    {
        Title = "Forbidden action",
        Description = "You are not authorized to grant/revoke access permissions",
        Code = nameof(ForbiddenAccessGranting)
    };
}