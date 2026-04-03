using HiTechStore.Core.Helpers;
using HiTechStore.Data;
using HiTechStore.Data.DTOs.Discount;
using HiTechStore.Data.Mapping;
using HiTechStore.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HiTechStore.Controllers;

[ApiController]
[Route("api/discounts/script")]
public class DiscountScriptController(
    IDiscountConditionScriptParser scriptParser,
    IConditionComponentTreeToLambdaExpression conditionToExpressionMapper,
    HiTechStoreDbContext dbContext
) : ControllerBase
{
    [HttpPost("check")]
    public async Task<ActionResult> CheckTheScript(ScriptCheckingDto scriptDto)
    {
        var script = scriptDto.Script!;

        var conditonTree = scriptParser.Parse(script);
        var conditionExpression = conditionToExpressionMapper.Map<Product>(conditonTree!);

        return Ok(
            await dbContext.Products.Where(
                conditionExpression
            ).ToListAsync()
        );
    }
}