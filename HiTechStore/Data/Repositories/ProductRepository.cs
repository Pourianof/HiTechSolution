using System.IO.Compression;
using System.Linq.Expressions;

using AutoMapper;

using HiTechStore.Core.Repositories;
using HiTechStore.Data.DTOs.Product;
using HiTechStore.Data.Queries;
using HiTechStore.Helpers.URLFilterQuery;
using HiTechStore.Models;

using Microsoft.EntityFrameworkCore;

namespace HiTechStore.Data.Repositories
{
    public class ProductRepository : Repository<Product, ProductDto, ProductQuery>, IProductRepository
    {
        public ProductRepository(HiTechStoreDbContext context, IMapper mapper) : base(context, mapper)
        {

        }

        private IQueryable<Product> BaseGettingQuery(IQueryable<Product> queryBuilder)
        {
            return queryBuilder.Select(p => new Product
            {
                ProductId = p.ProductId,
                Title = p.Title,
                AverageScore = p.Scores.Any()
                                 ? p.Scores.Average(s => (double?)s.Score)
                                 : 0.0,
                ScoreCounts = p.Scores.Count(),
                AuthorId = p.AuthorId,
                ComponentModels = p.ComponentModels,
                Author = p.Author,
                BrandModel = p.BrandModel,
                Category = p.Category,
                CategoryId = p.CategoryId,
                CreatedAt = p.CreatedAt,
                MyScore = p.MyScore,
                Description = p.Description,
                Price = p.Price,
                Media = p.Media,
                Properties = p.Properties
            });
        }

        protected override IQueryable<Product> GetAllQueryBuilder(IQueryable<Product> queryBuilder, ProductQuery? productQueryParams)
        {

            if (productQueryParams is not null)
            {
                if (productQueryParams.Category is not null)
                {
                    queryBuilder = queryBuilder.Where((p) => productQueryParams.Category.Value == p.CategoryId);
                }

                if (productQueryParams.FilterMaps.Count() > 0)
                {
                    // There are two types of keys:
                    // 1- Normal keys: single part keys which refer to a componentModel brandModel. 
                    //    Like gpu=Nvidia which gpu refer to component type and Nvidia refer to brand-model.
                    // 2- Multi-part keys: these keys are separated  by dot(.) character which parsed
                    //    as <componentType>.<propertyName>. Like "gpu.VRam=8"
                    var filterKeyTypeGroups = productQueryParams.FilterMaps
                    .GroupBy(
                        (filter) =>
                        filter.Key.Split('.', 2).Length);

                    var singlePartKeys = filterKeyTypeGroups.FirstOrDefault((g) => g.Key == 1)?.ToList() ?? [];
                    var multiPartKeys = filterKeyTypeGroups.FirstOrDefault((g) => g.Key == 2)?.ToList() ?? [];

                    IEnumerable<(string ComponentName, string PropertyName)> referedComponentProperties = multiPartKeys.Select(
                        (mpk) =>
                        {
                            var parts = mpk.Key.Split('.', 2, StringSplitOptions.RemoveEmptyEntries |
                                                            StringSplitOptions.TrimEntries);

                            return (ComponentName: parts[0], PropertyName: parts[1]);
                        }
                    );


                    var filters = multiPartKeys.Select(
                         (mpk) =>
                         {
                             var (key, filter) = mpk;
                             var parts = key.Split('.', 2, StringSplitOptions.RemoveEmptyEntries |
                                                             StringSplitOptions.TrimEntries);
                             var stringValue = filter.Value;
                             var numberValue = filter.GetValue<double>();
                             bool? booleanValue = string.Equals(stringValue, "true", StringComparison.OrdinalIgnoreCase) ? true :
                                                string.Equals(stringValue, "false", StringComparison.OrdinalIgnoreCase) ? false : null;
                             var dateValue = filter.GetValue<DateTime>();

                             return new ComponentFilter(parts[0], parts[1], new PropertyPossibleValues(stringValue, numberValue, dateValue, booleanValue));
                         }
                     );

                    // Improvement: use expression abstract tree to handle which type must eliminates based on
                    // if for that type the converted value is null

                    foreach (var filter in filters)
                    {
                        var (componentTypeName, propertyName, (stringValue, numberValue, dateValue, booleanValue)) = filter;

                        IEnumerable<string> desiredComponentBrandModels = singlePartKeys.Where((filter) => filter.Key == componentTypeName)
                                                            .Select(f => f.Value.GetValue<string>()?.ToLower())
                                                            .Where(modelName => !string.IsNullOrWhiteSpace(modelName))!;

                        queryBuilder = queryBuilder.Where(
                            (p) => p.ComponentModels.Any(
                                (cm) =>
                                    desiredComponentBrandModels.Contains(cm.BrandModel!.Brand!.NormalizedName) &&
                                    EF.Functions.ILike(componentTypeName, cm.ComponentType!.Name!) &&
                                    cm.Properties!.Any(
                                        (prop) => EF.Functions.ILike(prop.Property!.Name!, propertyName) &&
                                                (
                                                    prop.Property.PropertyType == PropertyType.Number ?
                                                        prop.Value!.ValueNumber == numberValue :
                                                    prop.Property.PropertyType == PropertyType.Boolean ?
                                                        prop.Value!.ValueBoolean == booleanValue :
                                                    prop.Property.PropertyType == PropertyType.String ?
                                                        prop.Value!.ValueString == stringValue :
                                                    false
                                                )

                                )
                            )
                        );
                    }
                }


            }
            return BaseGettingQuery(queryBuilder);
        }

        protected override IQueryable<Product> GetByIdAsyncQueryBuilder(IQueryable<Product> queryBuilder)
        {
            return BaseGettingQuery(queryBuilder);
        }

        public async Task<ProductDto?> GetByIdAsync(int id, string? userId)
        {
            var query = BaseGettingQuery(_dbSet).Where(p => p.ProductId == id);
            if (userId is string)
            {
                query.Where(p => p.AuthorId == userId);
            }
            return await Project(query).FirstOrDefaultAsync();

        }

        public override async Task Delete(int id)
        {
            await _dbSet.Where((p) => p.ProductId == id)
                  .ExecuteUpdateAsync(
                      (setter) =>
                          setter.SetProperty((prod) => prod.IsDeleled, true)
                  );

        }

        public override Task Delete(Product product)
        {
            product.IsDeleled = true;
            _context.Entry(product).Property((p) => p.IsDeleled).IsModified = true;
            return Task.CompletedTask;
        }
    }
}

record struct ComponentFilter(string ComponentName, string PropertyName, PropertyPossibleValues PossibleValues);
record struct PropertyPossibleValues(string? ValueString, double? ValueNumber, DateTime? ValueDateTime, bool? ValueBoolean);