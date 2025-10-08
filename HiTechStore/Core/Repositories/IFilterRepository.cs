using HiTechStore.Data.DTOs;

namespace HiTechStore.Core.Repositories;

public interface IFilterRepository
{
    Task<FilterDto> GetCategoryFiltersAsync(int categoryId);
    Task<FilterDto> GetProductsOveralFilters();
}