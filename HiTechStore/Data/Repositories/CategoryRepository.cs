using HiTechStore.Core.Repositories;
using HiTechStore.Models;

using Microsoft.EntityFrameworkCore;

namespace HiTechStore.Data.Repositories
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(HiTechStoreDbContext context) : base(context)
        {
        }

        protected override IQueryable<Category> GetAllQueryBuilder(IQueryable<Category> queryBuilder)
        {
            return queryBuilder.Include((c) => c.CategoryProperties);
        }
        public IEnumerable<Category> GetCategoriesByName(string name)
        {
            return _dbSet.Where(c => c.Name!.ToLower().Contains(name.ToLower())).ToList();
        }
    }
}