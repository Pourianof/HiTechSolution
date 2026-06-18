using AutoMapper;

using HiTechStore.Core;
using HiTechStore.Infrastructure.Data.DTOs.Brand;
using HiTechStore.Core.Models;

using Microsoft.AspNetCore.Mvc;

namespace HiTechStore.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BrandModelsController : ControllerBase
{
    private IUnitOfWork _unitOfWork { get; set; }
    private IMapper _mapper { get; set; }
    public BrandModelsController(IUnitOfWork unitOfWork, IMapper mapper) : base()
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BrandModelDto>>> GetAllBrandModels()
    {
        return Ok(await _unitOfWork.BrandModelRepository.GetPagedProjectedAsync());
    }

    [HttpPost]
    public async Task<ActionResult<BrandModelDto>> RegisterBrandModel([FromForm] BrandModelCreationDto brandModelCreationDto)
    {
        var brandModel = _mapper.Map<BrandModel>(brandModelCreationDto);
        var resultDto = _mapper.Map<BrandModelDto>(brandModel);

        if (brandModelCreationDto.Brand is not null)
        {
            var brandName = brandModelCreationDto.Brand.name;
            var dbBrand = await _unitOfWork.BrandRepository.GetByNameAsync(brandName!);

            if (dbBrand is null)
            {
                brandModel.Brand = new Brand
                {
                    Name = brandName,
                };
            }
            else
            {
                brandModel.BrandId = dbBrand.BrandId;
            }
        }
        else
        {
            var brand = await _unitOfWork.BrandRepository.GetByIdProjectedAsync(brandModel.BrandId);
            if (brand is null)
            {
                var problem = new ProblemDetails()
                {
                    Title = "Not found brand",
                    Detail = "specified brand id does not exist",
                    Status = StatusCodes.Status400BadRequest
                };
                return BadRequest(problem);
            }

            resultDto.BrandName = brand.Name;
        }

        await _unitOfWork.BrandModelRepository.AddAsync(brandModel);
        await _unitOfWork.Complete();

        resultDto.ModelId = brandModel.BrandModelId;

        return Ok(resultDto);
    }
}