using HiTechStore.Core.Common.Interfaces.Infra.Repositories;
using HiTechStore.Core.Models;

namespace HiTechStore.Core.Common.Interfaces.Infra;

public interface IPermissionRepository : IRepository<Permission>
{
    Task SeedSafely(IEnumerable<Permission> permissions);
    Task<IEnumerable<Permission>> GetUserPermissions(string userId);
    Task<IEnumerable<Permission>> GetPermissionsByCode(IEnumerable<string> permissionCodes);
}