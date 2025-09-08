using HiTechStore.Core.Repositories;
using HiTechStore.Models;

namespace HiTechStore.Data.Repositories
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(HiTechStoreDbContext context) : base(context)
        {
        }

        public IEnumerable<Category> GetCategoriesByName(string name)
        {
            return _dbSet.Where(c => c.Name!.ToLower().Contains(name.ToLower())).ToList();
        }
    }
}