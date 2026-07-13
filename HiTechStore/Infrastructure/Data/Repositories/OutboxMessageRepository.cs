
using AutoMapper;

using HiTechStore.Core.Common.Interfaces.Infra;
using HiTechStore.Core.Models;

using Microsoft.EntityFrameworkCore;

namespace HiTechStore.Infrastructure.Data.Repositories;

public class OutboxMessageRepository : Repository<OutboxMessage, Guid>
{
    public OutboxMessageRepository(HiTechStoreDbContext context, IMapper mapper) : base(context, mapper)
    {
    }

    public async Task<IEnumerable<OutboxMessage>> GetUnprocessedMessages()
    {
        return await _dbSet.Where(
            om => om.ProcessedAt == null
        ).ToListAsync();
    }

    public Task<int> Complete()
    {
        return _context.SaveChangesAsync();
    }
}