using AutoMapper;
using AutoMapper.QueryableExtensions;

using HiTechStore.Core.Repositories;
using HiTechStore.Data.DTOs;
using HiTechStore.Data.DTOs.Brand;
using HiTechStore.Data.DTOs.Component;
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

        public async Task<object?> GetFilters(int categoryId)
        {
            var brands = await _context.Products.Where((p) => p.CategoryId == categoryId && p.BrandModel != null)
                            .Select(p => p.BrandModel!.Brand)
                            .GroupBy(
                                b => new { b!.BrandId, b.Name }
                            ).Select(
                                (g) => new BrandFilterDto
                                {
                                    BrandId = g.Key.BrandId,
                                    Name = g.Key.Name,
                                    Frequency = g.Count()
                                }
                            ).OrderBy((b) => b.Frequency)
                            .ToListAsync();

            var properties = await _dbSet.Where((c) => c.CategoryId == categoryId)
                                .SelectMany((c) => c.Properties!)
                                .Select(p => new FilterPropertyDto
                                {
                                    Name = p.Name,
                                    PropertyId = p.PropertyId,
                                    Unit = p.Unit,
                                    TotalFrequency = p.ProductValues!.Count(),
                                    CommonValues = p.ProductValues!.GroupBy(
                                                (v) => new
                                                {
                                                    v.Value!.ValueNumber,
                                                    v.Value!.ValueString,
                                                    v.Value!.ValueDateTime,
                                                    v.Value!.ValueBoolean
                                                }
                                            ).Select(
                                                (g) => new PropertyCommomValueDto
                                                {
                                                    Value = p.PropertyType == PropertyType.Number ? g.Key.ValueNumber :
                                                            p.PropertyType == PropertyType.DateTime ? g.Key.ValueDateTime :
                                                            p.PropertyType == PropertyType.Boolean ? g.Key.ValueBoolean :
                                                            p.PropertyType == PropertyType.String ? g.Key.ValueString :
                                                            null,
                                                    Frequency = g.Count(),
                                                }
                                            )
                                })
                                .ToListAsync();
            var components = await _dbSet.Where((c) => c.CategoryId == categoryId)
                                .SelectMany((c) => c.Components!).Select(cmp => cmp.Component)
                                .Select((cmp) => new FilterComponentsDto
                                {
                                    ComponentId = cmp!.ComponentTypeId,
                                    Name = cmp!.Name,
                                    Properties = cmp!.Properties!.Select(
                                        p => new FilterPropertyDto
                                        {
                                            Name = p.Name,
                                            PropertyId = p.PropertyId,
                                            Unit = p.Unit,
                                            TotalFrequency = p.ComponentValues!.Count(),
                                            CommonValues = p.ComponentValues!.GroupBy(
                                                (v) => v.Value!.ValueNumber
                                            ).Select(
                                                (g) => new PropertyCommomValueDto
                                                {
                                                    Value = g.Key,
                                                    Frequency = g.Count(),
                                                }
                                            )
                                        }
                                    ),
                                })
                                .ToListAsync();

            return new FilterDto
            {
                Brands = brands,
                Properties = properties,
                Components = components
            };
        }
    }
}
