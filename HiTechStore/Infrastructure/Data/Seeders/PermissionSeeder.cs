using HiTechStore.Core;
using HiTechStore.Core.Models;

namespace HiTechStore.Infrastructure.Data.Seeders;

public static class PermissionSeeder
{
    public static async Task SeedPermissions(this IUnitOfWork unitOfWork)
    {
        await unitOfWork.PermissionRepository.SeedSafely(
             [
                 // Product
                 new Permission{
                    Code = Permissions.Product.Create,
                    Name = "Product creation",
                },
                new Permission{
                    Code = Permissions.Product.Delete,
                    Name = "Product deletion",
                },
                new Permission{
                    Code = Permissions.Product.Edit,
                    Name = "Product editing",
                },

                // Comment
                new Permission{
                    Code = Permissions.Comment.Moderate,
                    Name = "Comment moderation",
                },

                new Permission{
                    Code = Permissions.Access.Grant,
                    Name = "Granting access",
                },
             ]
         );
    }
}