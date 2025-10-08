using HiTechStore.Core;
using HiTechStore.Helpers.URLFilterQuery;

using Microsoft.AspNetCore.Mvc;

namespace HiTechStore.Controllers;


[ApiController]
[Route("[controller]")]
public class FiltersController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public FiltersController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public async Task<ActionResult> GetProductsFilters([ToQuery] FilterRequestQuery query)
    {
        if (query.Category?.Value is not null)
        {
            return Ok(await _unitOfWork.FilterRepository.GetCategoryFiltersAsync(query.Category.Value));
        }

        return Ok(await _unitOfWork.FilterRepository.GetProductsOveralFilters());
    }
}


public class FilterRequestQuery
{
    public QueryFilterItem<int>? Category { get; set; }
}