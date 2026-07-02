using HiTechStore.Core.Common.Interfaces.Infra.Repositories;
using HiTechStore.Core.Dto.Permission;
using HiTechStore.Core.Exceptions;
using HiTechStore.Core.Models;
using HiTechStore.Core.Services.Permission;

namespace HiTechStore.Core.Services.Discount;


public class DiscountPermissionHelper(IPermissionService permissionService, IDiscountCodeRepository discountrepository)
{
    private async Task<bool> IsOwner(string userId, int discountId)
    {
        var discount = await discountrepository.GetByIdProjectedAsync(discountId);

        if (discount is null)
        {
            throw new NotFoundException("Discount not found", $"Discount with id {discountId} not exists");
        }

        var isOwner = discount?.CreatorId == userId;

        return isOwner;
    }

    public async Task<bool> HasDiscountCreatePermission(string userId)
    {
        return await permissionService.HasAllPermissions(
            userId,
            [
                new UserPermissionDto(){
                    Code= Permissions.Discount.Create,
                }
            ]
        );
    }

    public async Task<bool> HasDiscountDeletePermission(string userId, int discountId)
    {
        return await permissionService.HasResourceAccess(
            userId,
            [
                new ResourceAccessCheck(){
                    Code= Permissions.Discount.Delete,
                    IsOwner = await IsOwner(userId, discountId)
                }
            ]
        );
    }

    public async Task<bool> HasDiscountEditPermission(string userId, int discountId)
    {
        return await permissionService.HasResourceAccess(
            userId,
            [
                new ResourceAccessCheck(){
                    Code= Permissions.Discount.Edit,
                    IsOwner = await IsOwner(userId, discountId)
                }
            ]
        );
    }

    public async Task<bool> HasPermissionToListAllDiscounts(string userId)
    {
        return await permissionService.HasAllPermissions(
            userId,
            [
                new UserPermissionDto(){
                    Code= Permissions.Discount.View,
                    Scope = PermissionScope.All
                }
            ]
        );
    }

    public async Task<bool> HasPermissionToListSelfDiscounts(string userId)
    {
        return await permissionService.HasAllPermissions(
            userId,
            [
                new UserPermissionDto(){
                    Code= Permissions.Discount.View,
                    Scope = PermissionScope.Self
                }
            ]
        );
    }

    public async Task<bool> HasPermissionToWorkWithDiscount(string userId)
    {
        return await permissionService.HasAnyPermissions(
            userId,
            [
                new UserPermissionDto(){
                    Code= Permissions.Discount.Create,
                },
                new UserPermissionDto(){
                    Code= Permissions.Discount.Edit,
                }
            ]
        );
    }
}