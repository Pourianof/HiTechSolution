using HiTechStore.Core;
using HiTechStore.Core.Models;

namespace HiTechStore.Infrastructure.Data.Seeders;

public static class AdminPermissionSeeder
{
    public static async Task SeedAminPermissions(this IUnitOfWork unitOfWork)
    {
        var admin = await unitOfWork.UserRepository.GetUserByUsernameAsync("admin");

        if (admin is null)
        {
            throw new InvalidOperationException("Admin user not defiend for assign permissions to it");
        }

        var allPermissions = await unitOfWork.PermissionRepository.GetAllAsync();

        foreach (var permission in allPermissions)
        {
            var exists = admin.Permissions.Any(
                perm => perm.Id == permission.Id
            );

            if (!exists)
            {
                var newPermission = new UserPermission
                {
                    UserId = admin.Id,
                    PermissionId = permission.Id,
                    GrantedAt = DateTime.UtcNow,
                    GrantedByUserId = admin.Id,
                };

                admin.Permissions.Add(newPermission);

                await unitOfWork.PermissionAuditRepository.AddAsync(
                      new()
                      {
                          Action = PermissionAction.Granted,
                          ActorUser = admin,
                          TargetUser = admin,
                          OccurredAt = DateTime.UtcNow,
                          Permission = permission
                      }
                  );
            }
        }

        await unitOfWork.Complete();
    }
}