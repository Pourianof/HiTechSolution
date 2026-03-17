using AutoMapper;

using HiTechStore.Core;
using HiTechStore.Core.Exceptions;
using HiTechStore.Core.Services.Discount;
using HiTechStore.Data.DTOs.DiscountCode;
using HiTechStore.Data.Queries;
using HiTechStore.Helpers.URLFilterQuery;
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

    [HttpGet("{name:alpha}")]
    public async Task<ActionResult<DiscountCode>> GetDiscountCode(string name)
    {
        var discountCode = await _unitOfWork.DiscountCodeRepository.GetDiscountCodeByNameProjectedAsync(name);

        if (discountCode is null)
        {
            return NotFound();
        }

        return Ok(discountCode);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<DiscountCode>> GetDiscountCodeByID(int id)
    {
        var discountCode = await _unitOfWork.DiscountCodeRepository.GetByIdProjectedAsync(id);

        if (discountCode is null)
        {
            return NotFound();
        }

        return Ok(discountCode);
    }

    [HttpGet]
    public async Task<ActionResult<DiscountCode>> GetAllDiscountCodes([ToQuery] DiscountQuery discountQuery)
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

    [HttpPatch("{id}")]
    public async Task<ActionResult<DiscountCodeDto>> UpdateDiscountCode(int id, [FromBody] DiscountCodeUpdateDto discountCodeUpdateDto)
    {
        var discountCode = await _disountService.UpdateDiscountCode(id, discountCodeUpdateDto, User);

        if (discountCode is null)
        {
            return NotFound();
        }

        return Ok(discountCode);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteDiscountCode(int id)
    {
        var deleted = await _disountService.DeleteDiscountCode(id);

        if (!deleted)
        {
            return NotFound();
        }

        return Ok();
    }

}
