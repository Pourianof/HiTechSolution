using HiTechStore.Core.Services.ProductScore;
using HiTechStore.Infrastructure.Data.DTOs.Product;
using HiTechStore.Presentation.Requests.ProductScore;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HiTechStore.Presentation.Controllers;

[ApiController]
[Route("api/products/{productId}")]
[Authorize]
public class ProductScoreController(IProductScoreService productScoreService) : ControllerBase
{
    [HttpPost("score/me")]
    public async Task<IActionResult> ScoreProduct(int productId, [FromBody] ProductScoreRequest score)
    {

        var newScore = await productScoreService.AddScoreForProduct(
            new()
            {
                ProductId = productId,
                Score = score.Score
            }
        );

        return Ok(newScore);
    }
}