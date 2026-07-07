
using AutoMapper;
using AutoMapper.QueryableExtensions;

using HiTechStore.Core.Common.Interfaces.Infra.Repositories;
using HiTechStore.Infrastructure.Data.DTOs.Component;
using HiTechStore.Core.Models;

using Microsoft.EntityFrameworkCore;

namespace HiTechStore.Infrastructure.Data.Repositories;

public class ComponentRepository : RepositoryWithIntegerId<ComponentType, ComponentTypeDto>, IComponentRepository
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

    public async Task<IEnumerable<ComponentModelDto>> GetComponentsModels(int componentId)
    {
        return await _context.ComponentType.Where((cmp) => cmp.ComponentTypeId == componentId)
                        .SelectMany(cmp => cmp.ComponentModels!)
                        .ProjectTo<ComponentModelDto>(_mapper.ConfigurationProvider)
                        .ToListAsync();
    }

    public async Task<IEnumerable<ComponentType>> GetByNameAsync(string name)
    {
        return await _dbSet.Where((c) => EF.Functions.ILike(c.Name!, name)).ToListAsync();
    }
}