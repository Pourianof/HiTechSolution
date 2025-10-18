using HiTechStore.Core;
using HiTechStore.Data.DTOs.Brand;

using Microsoft.AspNetCore.Mvc;

namespace HiTechStore.Controllers;

[ApiController]
[Route("[controller]")]
public class BrandModelsController : ControllerBase
{
    private IUnitOfWork _unitOfWork { get; set; }
    public BrandModelsController(IUnitOfWork unitOfWork) : base()
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BrandModelDto>>> GetAllBrandModels()
    {
        return Ok(await _unitOfWork.BrandModelRepository.GetAllAsync());
    }
}