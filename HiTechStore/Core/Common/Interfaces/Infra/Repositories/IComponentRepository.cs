using HiTechStore.Infrastructure.Data.DTOs.Component;
using HiTechStore.Core.Models;

namespace HiTechStore.Core.Common.Interfaces.Infra.Repositories;

public interface IComponentRepository : IRepository<ComponentType, ComponentTypeDto>
{
    Task<ComponentTypeDto?> GetSingleComponentOfCategoryById(int categoryId, int componentId);
    Task<IEnumerable<ComponentTypeDto>> GetComponentsOfCategory(int categoryId);
    Task<IEnumerable<ComponentModel>> GetComponentModelsOfCategory(int categoryId);
    Task<IEnumerable<ComponentModelDto>> GetComponentsModels(int categoryId);
    Task<IEnumerable<ComponentType>> GetByNameAsync(string name);
}