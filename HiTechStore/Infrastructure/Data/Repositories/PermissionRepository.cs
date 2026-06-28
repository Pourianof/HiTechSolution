
using AutoMapper;

using HiTechStore.Core.Common.Interfaces.Infra;
using HiTechStore.Core.Models;

using Microsoft.EntityFrameworkCore;

namespace HiTechStore.Infrastructure.Data.Repositories;


public class PermissionRepository : Repository<Permission>, IPermissionRepository
{
    public PermissionRepository(HiTechStoreDbContext context, IMapper mapper) : base(context, mapper)
    {
    }

    public async Task<IEnumerable<Permission>> GetPermissionsByCode(IEnumerable<string> permissionCodes)
    {
        return await _dbSet.Where(
            (perm) => permissionCodes.Contains(perm.Code)
        ).ToListAsync();
    }

    public async Task<IEnumerable<Permission>> GetUserPermissions(string userId)
    {
        return await _context.Users.Where(
            (u) => u.Id == userId
        ).SelectMany(
            (u) => u.Permissions
        ).Select(
            up => up.Permission!
        ).ToListAsync();
    }

    public async Task SeedSafely(IEnumerable<Permission> permissions)
    {
        foreach (var perm in permissions)
        {
            var permission = await _dbSet.FirstOrDefaultAsync(p => p.Code == perm.Code);

            if (permission is not null)
            {
                continue;
            }

            await AddAsync(perm);
        }
    }
}