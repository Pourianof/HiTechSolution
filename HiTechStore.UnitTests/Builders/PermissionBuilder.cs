using HiTechStore.Core.Models;

namespace HiTechStore.UnitTests.Builders;

public class PermissionBuilder
{
    private static int IdCounter = 1;
    public static UserPermission Access => new PermissionBuilder()
        .WithPermission(Permissions.Access.Grant)
        .Build();
    public static UserPermission ProductCreateSelf => new PermissionBuilder()
        .WithPermission(Permissions.Product.Create)
        .WithScope(PermissionScope.Self)
        .Build();
    public static UserPermission ProductCreateAll => new PermissionBuilder()
    .WithPermission(Permissions.Product.Create)
    .WithScope(PermissionScope.Self)
    .Build();

    private string? _permission;
    private PermissionScope _scope;
    private string? _grantBy, _targetUserId;

    public PermissionBuilder WithPermission(string permission)
    {
        _permission = permission;
        return this;
    }

    public PermissionBuilder WithScope(PermissionScope scope)
    {
        _scope = scope;

        return this;
    }

    public PermissionBuilder WithGrantBy(string userId)
    {
        _grantBy = userId;
        return this;
    }

    public PermissionBuilder WithTargetUser(string userId)
    {
        _targetUserId = userId;
        return this;
    }

    public UserPermission Build()
    {
        if (_permission is null)
            throw new InvalidDataException("you must invoke WithPermission to specify the permission");

        return new()
        {
            Permission = new()
            {
                Code = _permission,
                Id = 1
            },
            Id = IdCounter++,
            Scope = _scope,
            GrantedByUserId = _grantBy,
            UserId = _targetUserId
        };
    }
}