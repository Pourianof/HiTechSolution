using HiTechStore.Core.Dto.Permission;
using HiTechStore.Core.Helpers.Result;

namespace HiTechStore.Core.Services.Permission;

public interface IPermissionService
{
    Task<Result<IEnumerable<Models.Permission>>> ModifyPermissions(ModifyPermissionDto modifyPermissionDto);
}