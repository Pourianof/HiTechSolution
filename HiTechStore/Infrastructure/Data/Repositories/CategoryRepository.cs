using AutoMapper;

using HiTechStore.Core.Common.Interfaces.Infra.Repositories;
using HiTechStore.Infrastructure.Data.DTOs;
using HiTechStore.Infrastructure.Data.Queries;
using HiTechStore.Core.Models;

using Microsoft.EntityFrameworkCore;

namespace HiTechStore.Infrastructure.Data.Repositories
{
    public class CategoryRepository : RepositoryWithIntegerId<Category, CategoryDTO>, ICategoryRepository
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

        public async Task<IEnumerable<ComponentModel>> GetModelsOfCategory(int categoryId, IEnumerable<int> modelIds)
        {
            return await _context.Categories.Where(c => c.CategoryId == categoryId)
                        .SelectMany((cc) => cc.Components!)
                        .Select(ct => ct.Component)
                        .SelectMany((c) => c!.ComponentModels!)
                        .Where(cm => modelIds.Contains(cm.ComponentModelId))
                        // .Include(cm => cm.ComponentType)
                        .ToListAsync();
        }
    }
}
