
using AutoMapper;

using HiTechStore.Core.Repositories;
using HiTechStore.Models;

using Microsoft.EntityFrameworkCore;

namespace HiTechStore.Data.Repositories;

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

