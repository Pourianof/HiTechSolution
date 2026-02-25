using AutoMapper;

using HiTechStore.Core;
using HiTechStore.Core.Exceptions;
using HiTechStore.Core.Services;
using HiTechStore.Data.DTOs.DiscountCode;
using HiTechStore.Data.Queries;
using HiTechStore.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HiTechStore.Controllers;

[Route("api/discounts/codes")]
[ApiController]
[Authorize]
public class DicountCodesController(
    IUnitOfWork unitOfWork,
    IDiscountService disountService,
    IMapper mapper) : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IDiscountService _disountService = disountService;
    private readonly IMapper _mapper = mapper;

    [HttpGet("random-code")]
    public IActionResult GetRandomCode()
    {
        var code = _disountService.GenerateRanomCode();

        return Ok(new { Code = code });
    }

    [HttpGet("{name:required}")]
    public async Task<ActionResult<DiscountCode>> GetDiscountCode(string name)
    {
        var discountCode = await _unitOfWork.DiscountCodeRepository.GetDiscountCodeByNameProjectedAsync(name);

        if (discountCode is null)
        {
            return NotFound();
        }

        return Ok(discountCode);
    }

    [HttpGet]
    public async Task<ActionResult<DiscountCode>> GetAllDiscountCodes([FromQuery] DiscountQuery discountQuery)
    {
        return Ok(await _disountService.GetAllDiscountCodes(discountQuery));
    }

    [HttpPost]
    public async Task<ActionResult> CreateCode([FromBody] DiscountCodeCreationDto? discountCodeCreationDto)
    {
        if (discountCodeCreationDto is null)
        {
            return BadRequest(
                new ProblemDetails
                {
                    Title = "Empty body",
                    Detail = "No discount model specified",
                }
            );
        }

        if (discountCodeCreationDto.StartTime > discountCodeCreationDto.EndTime)
        {
            ModelState.AddModelError(nameof(DiscountCodeCreationDto.StartTime), "StartTime cannot be greater than EndTime");

            return ValidationProblem(ModelState);
        }

        try
        {
            var newDiscountModel = _mapper.Map<DiscountCode>(discountCodeCreationDto);
            var discountCode = await _disountService.RegisterDiscountCode(
                newDiscountModel
            );

            return Ok(discountCode);
        }
        catch (ModelException ex)
        {
            ModelState.AddModelError(ex.FieldName, ex.Message);

            return ValidationProblem(ModelState);
        }
        catch (Core.Exceptions.ApplicationException ex)
        {
            var problemDetails = new ProblemDetails
            {
                Title = ex.Title,
                Detail = ex.Message
            };
            return BadRequest(problemDetails);
        }
    }

}
