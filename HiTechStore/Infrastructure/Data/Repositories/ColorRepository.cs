
using AutoMapper;

using HiTechStore.Core.Common.Interfaces.Infra.Repositories;
using HiTechStore.Core.Models;

using Microsoft.EntityFrameworkCore;

namespace HiTechStore.Infrastructure.Data.Repositories;

public class ColorRepository : Repository<Color>, IColorRepository
{
    public ColorRepository(HiTechStoreDbContext context, IMapper mapper) : base(context, mapper)
    {
    }

    public Task<Color?> GetColorByNameAsync(string name)
    {
        return _dbSet.Where(
            (c) => EF.Functions.ILike(name, c.Name!)
        ).FirstOrDefaultAsync();
    }
}

