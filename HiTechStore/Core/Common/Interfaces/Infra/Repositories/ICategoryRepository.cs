using HiTechStore.Infrastructure.Data.DTOs;
using HiTechStore.Core.Models;

namespace HiTechStore.Core.Common.Interfaces.Infra.Repositories
{
    public interface ICategoryRepository : IRepositoryWithIntegerId<Category, CategoryDTO>
    {
        IEnumerable<Category> GetCategoriesByName(string name);
        Task<IEnumerable<Property>> GetCategoryPropertiesAsync(int categoryId);
        Task<IEnumerable<ComponentModel>> GetModelsOfCategory(int categoryId, IEnumerable<int> modelIds);
    }
}
