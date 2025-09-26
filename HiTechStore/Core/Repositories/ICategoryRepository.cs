using HiTechStore.Data.DTOs;
using HiTechStore.Models;

namespace HiTechStore.Core.Repositories
{
    public interface ICategoryRepository : IRepository<Category, CategoryDTO>
    {
        IEnumerable<Category> GetCategoriesByName(string name);
        Task<IEnumerable<Property>> GetCategoryPropertiesAsync(int categoryId);
    }
}
