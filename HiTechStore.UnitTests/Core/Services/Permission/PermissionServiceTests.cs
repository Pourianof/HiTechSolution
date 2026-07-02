using HiTechStore.Core.Dto.Permission;
using HiTechStore.Core.Models;
using HiTechStore.Core.Services.Permission;
using HiTechStore.UnitTests.Builders;
using HiTechStore.UnitTests.TestDoubles;

using Moq;

namespace HiTechStore.UnitTests.Core.Services.Permission;

public class PermissionServiceTests
{
    [Fact]
    public async Task ModifyPermissions_WhenGrantingWithDifferentScope_ReplacesExistingGrantScope()
    {
        var factory = new PermissionServiceTestFactory();

        factory.PermissionRepository
            .Setup(x => x.GetUserPermissions(factory.ActorUser.Id))
            .ReturnsAsync([
                new UserPermissionDto { Code = Permissions.Product.Create, Scope = PermissionScope.All }
            ]);

        factory.ActorUser.Permissions = [
            new PermissionBuilder()
                .WithPermission(Permissions.Access.Grant)
                .WithGrantBy("actor")
                .Build(),
            new PermissionBuilder()
                .WithPermission(Permissions.Product.Create)
                .WithGrantBy("actor")
                .Build()
        ];
        var service = factory.Create();

        // Action
        var result = await service.ModifyPermissions(new ModifyPermissionDto
        {
            TargetUserId = factory.TargetUser.Id,
            Permissions =
            [
                new TargetPermissionDto
                {
                    PermissionCode = Permissions.Product.Create,
                    Action = PermissionModificationAction.Grant,
                    Scope = PermissionScope.All
                }
            ]
        });

        // Assert
        Assert.True(result.IsValid);
        Assert.Single(factory.TargetUser.Permissions!);
        Assert.Equal(PermissionScope.All, factory.TargetUser.Permissions!.Single().Scope);
        factory.AuditRepository.Verify(x => x.AddAsync(It.IsAny<PermissionAudit>()), Times.Once);
    }


    [Fact]
    public async Task ModifyPermissions_WhenUserIdIsNotExist_UserNotFoundError()
    {
        // Assert
        var service = new PermissionServiceTestFactory().Create();

        // Action
        var result = await service.ModifyPermissions(new ModifyPermissionDto
        {
            TargetUserId = "some-random-not-existing-id",
            Permissions = []
        });

        // Assert
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Contains(result.Errors, err => err.Code == nameof(PermissionErrors.UserNotFound));
    }

    [Fact]
    public async Task ModifyPermissions_ShouldReturnFailure_WhenNotAdminActorUserTryToUpdateRoledTargetUser()
    {
        // Arrange
        var factory = new PermissionServiceTestFactory();
        factory.TargetUser.Roles = [IdentityRoles.Manager];

        var service = factory.Create();


        // Action
        var result = await service.ModifyPermissions(new ModifyPermissionDto
        {
            TargetUserId = factory.TargetUser.Id,
            Permissions = []
        });


        // Assert
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Contains(result.Errors, err => err.Code == nameof(PermissionErrors.NotAuthorizedToModifyTargetUsersPermissions));
    }

    [Fact]
    public async Task ModifyPermissions_WhenAdminTryToUpdateAdminUser_ShouldReturnFailure()
    {
        // Arrange
        var factory = new PermissionServiceTestFactory();

        factory.TargetUser.Roles = [IdentityRoles.Admin];
        factory.ActorUser.Roles = [IdentityRoles.Admin];

        var service = factory.Create();

        // Action
        var result = await service.ModifyPermissions(new ModifyPermissionDto
        {
            TargetUserId = factory.TargetUser.Id,
            Permissions = []
        });


        // Assert
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Contains(result.Errors, err => err.Code == nameof(PermissionErrors.AdminModifyAdminRestriction));
    }

    [Fact]
    public async Task ModifyPermissions_WhenActorHaveNotGrantPermission_ShouldReturnFailure()
    {
        // Arrange
        var factory = new PermissionServiceTestFactory();

        factory.ActorUser.Permissions = [];

        var service = factory.Create();


        // Action
        var result = await service.ModifyPermissions(new ModifyPermissionDto
        {
            TargetUserId = factory.TargetUser.Id,
            Permissions = []
        });


        // Assert
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Contains(result.Errors, err => err.Code == nameof(PermissionErrors.GrantPermissionRequiredGrantAccess));
    }

    [Fact]
    public async Task ModifyPermissions_WhenSpecifiedPermissionNotExist_ShouldReturnFailure()
    {
        // Arrange
        var randomCode = "random-not-existing-permission";

        var factory = new PermissionServiceTestFactory();

        factory.ActorUser.Permissions = [
            PermissionBuilder.Access,
        ];

        var service = factory.Create();


        // Action
        var result = await service.ModifyPermissions(new ModifyPermissionDto
        {
            TargetUserId = factory.TargetUser.Id,
            Permissions = [
                new (){
                    PermissionCode = randomCode,
                    Scope= PermissionScope.All
                }
            ]
        });


        // Assert
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Contains(result.Errors, err => err.Code == nameof(PermissionErrors.InvalidPermission));
    }

    [Fact]
    public async Task ModifyPermissions_WhenNotAdminActorGrantingAccessPermission_ShouldReturnFailure()
    {
        // Arrange
        var factory = new PermissionServiceTestFactory();
        factory.ActorUser.Permissions = [
          PermissionBuilder.Access
        ];

        var service = factory.Create();

        // Action
        var result = await service.ModifyPermissions(new ModifyPermissionDto
        {
            TargetUserId = factory.TargetUser.Id,
            Permissions = [
                new (){
                    PermissionCode = Permissions.Access.Grant,
                    Scope= PermissionScope.All
                }
            ]
        });


        // Assert
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Contains(result.Errors, err => err.Code == nameof(PermissionErrors.ForbiddenAccessGranting));
    }

    [Fact]
    public async Task ModifyPermissions_WhenActorNotHaveGrantingPermissionHimself_ShouldReturnFailure()
    {
        // Arrange
        var factory = new PermissionServiceTestFactory();
        factory.ActorUser.Permissions = [
            PermissionBuilder.Access
        ];

        var service = factory.Create();

        // Action
        var result = await service.ModifyPermissions(new ModifyPermissionDto
        {
            TargetUserId = factory.TargetUser.Id,
            Permissions = [
                new (){
                    PermissionCode = Permissions.Product.Create,
                    Scope= PermissionScope.All
                }
            ]
        });


        // Assert
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Contains(result.Errors, err => err.Code == nameof(PermissionErrors.CannotGrantPermissionYouDoNotHave));
    }

    [Fact]
    public async Task ModifyPermissions_WhenActorGrantsScopeBeyondHisOwn_ShouldReturnFailure()
    {
        // Arrange
        var factory = new PermissionServiceTestFactory();
        factory.ActorUser.Permissions = [
            PermissionBuilder.Access,
            PermissionBuilder.ProductCreateSelf
        ];

        var service = factory.Create();

        // Action
        var result = await service.ModifyPermissions(new ModifyPermissionDto
        {
            TargetUserId = factory.TargetUser.Id,
            Permissions = [
                new (){
                    PermissionCode = Permissions.Product.Create,
                    Scope= PermissionScope.All
                }
            ]
        });


        // Assert
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Contains(result.Errors, err => err.Code == nameof(PermissionErrors.CannotGrantPermissionScopeWhichHigherThanYou));
    }

    [Fact]
    public async Task ModifyPermissions_WhenActorRevokeScopeBeyondHisOwn_ShouldReturnFailure()
    {
        // Arrange
        var factory = new PermissionServiceTestFactory();
        factory.ActorUser.Permissions = [
            PermissionBuilder.Access,
            PermissionBuilder.ProductCreateSelf
        ];

        var service = factory.Create();

        // Action
        var result = await service.ModifyPermissions(new ModifyPermissionDto
        {
            TargetUserId = factory.TargetUser.Id,
            Permissions = [
                new (){
                    PermissionCode = Permissions.Product.Create,
                    Scope= PermissionScope.All
                }
            ]
        });


        // Assert
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Contains(result.Errors, err => err.Code == nameof(PermissionErrors.CannotGrantPermissionScopeWhichHigherThanYou));
    }

    [Fact]
    public async Task ModifyPermissions_WhenActorRevokeScope_ShouldRemoveFromUsersPermissions()
    {
        // Arrange
        var factory = new PermissionServiceTestFactory();
        factory.ActorUser.Permissions = [
            PermissionBuilder.Access,
            PermissionBuilder.ProductCreateSelf
        ];

        factory.TargetUser.Permissions = [
            PermissionBuilder.ProductCreateSelf
        ];

        var service = factory.Create();

        // Action
        var result = await service.ModifyPermissions(new ModifyPermissionDto
        {
            TargetUserId = factory.TargetUser.Id,
            Permissions = [
                new (){
                    PermissionCode = Permissions.Product.Create,
                    Scope= PermissionScope.Self,
                    Action= PermissionModificationAction.Revoke
                }
            ]
        });

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(factory.TargetUser.Permissions);
    }

    [Fact]
    public async Task ModifyPermissions_WhenTargetUserHasAccessPermission_ShouldLockHisPermissionsList()
    {
        // Arrange
        var factory = new PermissionServiceTestFactory();
        factory.ActorUser.Permissions = [
            PermissionBuilder.Access,
            PermissionBuilder.ProductCreateSelf
        ];

        factory.TargetUser.Permissions = [
            PermissionBuilder.Access,
            PermissionBuilder.ProductCreateSelf
        ];

        var service = factory.Create();

        // Action
        var result = await service.ModifyPermissions(new ModifyPermissionDto
        {
            TargetUserId = factory.TargetUser.Id,
            Permissions = [
                new (){
                    PermissionCode = Permissions.Product.Create,
                    Scope= PermissionScope.Self,
                }
            ],
        });

        // Assert
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Contains(result.Errors, err => err.Code == nameof(PermissionErrors.LockingPermissionListForAccessGrantedTargetUser));
    }
}
