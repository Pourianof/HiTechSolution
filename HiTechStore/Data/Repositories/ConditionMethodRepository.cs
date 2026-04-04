
using AutoMapper;

using HiTechStore.Data;
using HiTechStore.Data.Repositories;
using HiTechStore.Models;

using Microsoft.EntityFrameworkCore;

namespace HiTechStore.Core.Repositories;


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