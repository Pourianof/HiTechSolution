using HiTechStore.Core;
using HiTechStore.Core.Common.Interfaces.Infra;
using HiTechStore.Core.Common.Interfaces.Presentation;
using HiTechStore.Core.Models;
using HiTechStore.Core.Services.Authorization;
using HiTechStore.Core.Services.Permission;
using HiTechStore.UnitTests.Builders;

using Moq;

namespace HiTechStore.UnitTests.TestDoubles;

public class PermissionServiceTestFactory
{
    public Mock<IPermissionRepository> PermissionRepository = new();
    public Mock<IPermissionAuditRepository> AuditRepository = new();
    public Mock<IAuthorizationService> AuthService = new();
    public Mock<ICurrentUserProvider> CurrentUser = new();

    public User ActorUser = new() { Id = "actor" };
    public User TargetUser = new() { Id = "target" };

    public PermissionService Create()
    {
        var uow = new Mock<IUnitOfWork>();
        CurrentUser.SetupGet(x => x.UserId).Returns(ActorUser.Id);

        uow.SetupGet(x => x.PermissionRepository).Returns(PermissionRepository.Object);
        uow.SetupGet(x => x.PermissionAuditRepository).Returns(AuditRepository.Object);

        AuthService
            .Setup(x => x.GetUserByIdAsync(ActorUser.Id))
            .ReturnsAsync(ActorUser);

        AuthService
            .Setup(x => x.GetUserByIdAsync(TargetUser.Id))
            .ReturnsAsync(TargetUser);

        PermissionRepository
            .Setup(x => x.GetPermissionsByCode(It.IsAny<IEnumerable<string>>()))
            .ReturnsAsync((IEnumerable<string> codes) => new List<Permission>
            {
                PermissionBuilder.Access.Permission!,
                PermissionBuilder.ProductCreateSelf.Permission!
            }.Where(
                perm => codes.Any(code => code == perm.Code)
            )
        );

        uow.Setup(x => x.Complete()).ReturnsAsync(1);


        return new PermissionService(
            uow.Object,
            AuthService.Object,
            CurrentUser.Object
        );
    }
}