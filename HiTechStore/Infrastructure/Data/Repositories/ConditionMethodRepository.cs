
using AutoMapper;

using HiTechStore.Infrastructure.Data;
using HiTechStore.Infrastructure.Data.Repositories;
using HiTechStore.Core.Models;

using Microsoft.EntityFrameworkCore;

namespace HiTechStore.Core.Common.Interfaces.Infra.Repositories;


public class ConditionMethodRepository : Repository<ConditionMethod>, IConditionMethodRepository
{
    public ConditionMethodRepository(HiTechStoreDbContext context, IMapper mapper) : base(context, mapper)
    {
    }

    public async Task AddAllSafe(IEnumerable<ConditionMethod> conditionMethods)
    {
        foreach (var condMethod in conditionMethods)
        {
            if (await _context.ConditionMethods.AnyAsync(c => c.Name == condMethod.Name))
            {
                continue;
            }

            await AddAsync(condMethod);
        }
        await _context.SaveChangesAsync();
    }
}