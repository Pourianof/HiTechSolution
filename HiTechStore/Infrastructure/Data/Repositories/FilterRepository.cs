using HiTechStore.Core.Common.Interfaces.Infra.Repositories;
using HiTechStore.Infrastructure.Data.DTOs;
using HiTechStore.Core.Models;

using Microsoft.EntityFrameworkCore;

namespace HiTechStore.Infrastructure.Data.Repositories;

public class FilterRepository : IFilterRepository
{
    private HiTechStoreDbContext _context { get; }
    public FilterRepository(HiTechStoreDbContext context)
    {
        _context = context;
    }

    private async Task<List<BrandFilterDto>?> ProvideBrandStats(int? categoryId = null)
    {
        var brandsQuery = _context.Products.Where((p) => p.BrandModel != null);

        if (categoryId is not null)
        {
            brandsQuery = brandsQuery.Where((p) => p.CategoryId == categoryId);
        }

        return await brandsQuery.Select(p => p.BrandModel!.Brand)
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
    }

    private async Task<ProductsPriceRangeDto?> ProvideProductsPriceRange(int? categoryId = null)
    {
        var priceQueryBuilder = _context.Products.AsQueryable();

        if (categoryId is not null)
        {
            priceQueryBuilder = priceQueryBuilder.Where((p) => p.CategoryId == categoryId && p.BrandModel != null);
        }

        return await priceQueryBuilder.GroupBy((p) => 1)
        .Select((g) => new
        ProductsPriceRangeDto
        {
            Max = g.Max(p => p.Variations.Max(v => v.Price)),
            Min = g.Min(p => p.Variations.Min(v => v.Price))
        }).FirstOrDefaultAsync();
    }
    public async Task<FilterDto> GetCategoryFiltersAsync(int categoryId)
    {
        var brands = await ProvideBrandStats(categoryId);

        var properties = await _context.Categories.Where((c) => c.CategoryId == categoryId)
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
        var components = await _context.Categories.Where((c) => c.CategoryId == categoryId)
                            .SelectMany((c) => c.Components!).Select(cmp => cmp.Component)
                            .Select((cmp) => new FilterComponentsDto
                            {
                                ComponentTypeId = cmp!.ComponentTypeId,
                                Name = cmp!.Name,
                                CommonBrands = cmp.ComponentModels!.Select(
                                    (model) => model.BrandModel!.Brand
                                ).GroupBy(
                                    b => new { b!.BrandId, b.Name }
                                ).Select(
                                    (g) => new BrandFilterDto
                                    {
                                        BrandId = g.Key.BrandId,
                                        Name = g.Key.Name,
                                        Frequency = g.Count()
                                    }
                                )
                                .OrderBy((b) => b.Frequency).ToList(),
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
                            }).ToListAsync();
        var productsPriceRange = await ProvideProductsPriceRange();


        return new FilterDto
        {
            Brands = brands,
            Properties = properties,
            Components = components,
            PriceRange = productsPriceRange
        };
    }

    public async Task<FilterDto> GetProductsOveralFilters()
    {
        var brands = await ProvideBrandStats();

        var productsPriceRange = await ProvideProductsPriceRange();

        return new FilterDto() { PriceRange = productsPriceRange, Brands = brands };
    }

}