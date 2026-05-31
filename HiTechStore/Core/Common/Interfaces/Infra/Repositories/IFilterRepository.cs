using HiTechStore.Infrastructure.Data.DTOs;

namespace HiTechStore.Core.Common.Interfaces.Infra.Repositories;

public interface IFilterRepository
{
    Task<FilterDto> GetCategoryFiltersAsync(int categoryId);
    Task<FilterDto> GetProductsOveralFilters();
}