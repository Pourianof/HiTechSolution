using AutoMapper;

using HiTechStore.Core.Repositories;
using HiTechStore.Data.DTOs;
using HiTechStore.Data.Queries;
using HiTechStore.Models;

using Microsoft.EntityFrameworkCore;

namespace HiTechStore.Data.Repositories
{
    public class CategoryRepository : Repository<Category, CategoryDTO>, ICategoryRepository
    {
        public CategoryRepository(HiTechStoreDbContext context, IMapper mapper) : base(context, mapper)
        {
        }

        protected override IQueryable<Category> GetAllQueryBuilder(IQueryable<Category> queryBuilder, BaseQuery? queryParams)
        {
            return queryBuilder.Include((c) => c.Properties)
                        .Include((c) => c.Components)!.ThenInclude((cmp) => cmp.Component);
        }
        public IEnumerable<Category> GetCategoriesByName(string name)
        {
            return _dbSet.Where(c => c.Name!.ToLower().Contains(name.ToLower())).ToList();
        }

        public async Task<IEnumerable<Property>> GetCategoryPropertiesAsync(int categoryId)
        {
            return (await _dbSet.Include((c) => c.Properties).Where((c) => c.CategoryId == categoryId)
                        .Select((c) => c.Properties).FirstAsync()) ?? new List<Property>();
        }

    }
}