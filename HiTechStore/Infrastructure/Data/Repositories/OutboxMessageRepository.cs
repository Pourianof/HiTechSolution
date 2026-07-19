
using AutoMapper;

using HiTechStore.Core.Models;

using Microsoft.EntityFrameworkCore;

namespace HiTechStore.Infrastructure.Data.Repositories;

public class OutboxMessageRepository : Repository<OutboxMessage, Guid>
{
    public bool HasPendingMessages { get; private set; } = false;

    public OutboxMessageRepository(HiTechStoreDbContext context, IMapper mapper) : base(context, mapper)
    {
    }
    public override async Task AddAsync(OutboxMessage entity)
    {
        await base.AddAsync(entity);

        HasPendingMessages = true;

        return;
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

    public void Reset()
    {
        HasPendingMessages = false;
    }
}