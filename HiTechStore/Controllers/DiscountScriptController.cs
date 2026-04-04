using HiTechStore.Core.Services.Discount;
using HiTechStore.Data.DTOs.Discount;

using Microsoft.AspNetCore.Mvc;

namespace HiTechStore.Controllers;

[ApiController]
[Route("api/discounts/script")]
public class DiscountScriptController(
    IDiscountService discountService
) : ControllerBase
{
    [HttpPost("check")]
    public async Task<ActionResult> CheckTheScript(ScriptCheckingDto scriptDto)
    {
        var script = scriptDto.Script!;

        var parseResult = await discountService.GetConditionScriptProducts(script);
        if (!parseResult.Succeed)
        {
            var promblem = new ProblemDetails
            {
                Title = "Failed interpreting scripts",
                Detail = parseResult.Message
            };

            return BadRequest(promblem);
        }


        return Ok(
            parseResult.ResultedProducts
        );
    }
}