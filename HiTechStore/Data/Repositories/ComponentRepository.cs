
using AutoMapper;
using AutoMapper.QueryableExtensions;

using HiTechStore.Core.Repositories;
using HiTechStore.Data.DTOs.Component;
using HiTechStore.Models;

using Microsoft.EntityFrameworkCore;

namespace HiTechStore.Data.Repositories;

public class ComponentRepository : Repository<ComponentType, ComponentTypeDto>, IComponentRepository
{
    public ComponentRepository(HiTechStoreDbContext context, IMapper mapper) : base(context, mapper)
    {
    }

    public Task<IEnumerable<ComponentModel>> GetComponentModelsOfCategory(int categoryId)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<ComponentTypeDto>> GetComponentsOfCategory(int categoryId)
    {
        return await _context.ComponentType.Where((cmp) => cmp.Categories!.Any(c => c.CategoryId == categoryId))
                        .ProjectTo<ComponentTypeDto>(_mapper.ConfigurationProvider).ToListAsync();
    }

    public async Task<ComponentTypeDto?> GetSingleComponentOfCategoryById(int categoryId, int componentId)
    {
        return await _context.ComponentType.Where((cmp) => cmp.ComponentTypeId == componentId && cmp.Categories!.Any(c => c.CategoryId == categoryId))
                        .ProjectTo<ComponentTypeDto>(_mapper.ConfigurationProvider).FirstOrDefaultAsync();
    }
}