using HiTechStore.Core;
using HiTechStore.Helpers.URLFilterQuery;

using Microsoft.AspNetCore.Mvc;

namespace HiTechStore.Presentation.Controllers;


[ApiController]
[Route("api/[controller]")]
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
        var categoryId = query.Category?.GetValue<int>(QueryOperator.Equal);
        if (categoryId is not null)
        {
            return Ok(await _unitOfWork.FilterRepository.GetCategoryFiltersAsync(categoryId.Value));
        }

        return Ok(await _unitOfWork.FilterRepository.GetProductsOveralFilters());
    }
}


public class FilterRequestQuery
{
    public QueryFilterItem? Category { get; set; }
}