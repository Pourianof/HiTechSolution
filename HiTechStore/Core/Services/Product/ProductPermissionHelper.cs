using HiTechStore.Core.Common.Interfaces.Infra.Repositories;
using HiTechStore.Core.Dto.Permission;
using HiTechStore.Core.Exceptions;
using HiTechStore.Core.Models;
using HiTechStore.Core.Services.Permission;

namespace HiTechStore.Core.Services.Product;

public class ProductPermissionHelper(IPermissionService permissionService, IProductRepository productRepository)
{
    private async Task<bool> IsOwner(string userId, int productId)
    {
        var product = await productRepository.GetByIdAsync(productId, default);

        if (product is null)
        {
            throw new NotFoundException("Product not found", $"Product with id {productId} not exists");
        }

        var isOwner = product?.AuthorId == userId;

        return isOwner;
    }

    public async Task<bool> HasProductCreatePermission(string userId)
    {
        return await permissionService.HasAllPermissions(
            userId,
            [
                new UserPermissionDto(){
                    Code= Permissions.Product.Create,
                }
            ]
        );
    }

    public async Task<bool> HasProductDeletePermission(string userId, int productId)
    {
        return await permissionService.HasResourceAccess(
            userId,
            [
                new ResourceAccessCheck(){
                    Code= Permissions.Product.Delete,
                    IsOwner = await IsOwner(userId, productId)
                }
            ]
        );
    }

    public async Task<bool> HasProductEditPermission(string userId, int productId)
    {
        return await permissionService.HasResourceAccess(
            userId,
            [
                new ResourceAccessCheck(){
                    Code= Permissions.Product.Edit,
                    IsOwner = await IsOwner(userId, productId)
                }
            ]
        );
    }
}