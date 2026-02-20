using HiTechStore.Core;
using HiTechStore.Core.Services;
using HiTechStore.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HiTechStore.Controllers;

[Route("api/discounts/codes")]
[ApiController]
[Authorize]
public class DicountCodesController(IUnitOfWork unitOfWork, IDiscountService disountService) : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IDiscountService _disountService = disountService;

    [HttpGet("random-code")]
    public IActionResult GetRandomCode()
    {
        var code = _disountService.GenerateRanomCode();

        return Ok(new { Code = code });
    }

    [HttpGet("{name:required}")]
    public async Task<ActionResult<DiscountCode>> GetDiscountCode(string name)
    {
        var discountCode = await _unitOfWork.DiscountCodeRepository.GetDiscountCodeByNameAsync(name);

        if (discountCode is null)
        {
            return NotFound();
        }

        return Ok(discountCode);
    }


}

