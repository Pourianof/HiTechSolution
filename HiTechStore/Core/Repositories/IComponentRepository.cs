using HiTechStore.Data.DTOs.Component;
using HiTechStore.Models;

namespace HiTechStore.Core.Repositories;

public interface IComponentRepository : IRepository<ComponentType, ComponentTypeDto>
{
    Task<ComponentTypeDto?> GetSingleComponentOfCategoryById(int categoryId, int componentId);
    Task<IEnumerable<ComponentTypeDto>> GetComponentsOfCategory(int categoryId);
    Task<IEnumerable<ComponentModel>> GetComponentModelsOfCategory(int categoryId);
    Task<IEnumerable<ComponentModelDto>> GetComponentsModels(int categoryId);
}