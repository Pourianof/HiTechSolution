using AutoMapper;

using HiTechStore.Core.Common.Interfaces.Infra;
using HiTechStore.Core.Models;

namespace HiTechStore.Infrastructure.Data.Repositories;

public class PermissionAuditRepository : Repository<PermissionAudit>, IPermissionAuditRepository
{
    public PermissionAuditRepository(HiTechStoreDbContext context, IMapper mapper) : base(context, mapper)
    {
    }
}