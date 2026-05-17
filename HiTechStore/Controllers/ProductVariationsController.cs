using AutoMapper;

using HiTechStore.Core.Dto.ProductVariation;
using HiTechStore.Core.Services.ProductVariation;
using HiTechStore.Presentation.Requests.ProductVariation;

using Microsoft.AspNetCore.Mvc;

namespace HiTechStore.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductVariationsController(
    IProductVariationService productVariationService,
    IMapper mapper
) : ControllerBase
{
    [HttpPatch("{id}")]
    public async Task<ActionResult> UpdateVariationDetails(int id, UpdateVariationDetailsRequest updateRequest)
    {
        var result = await productVariationService.UpdateDetails(id, mapper.Map<UpdateProductVariationDetailsDto>(updateRequest));

        if (!result)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Update failed",
                Detail = "Update failed with unknown reason"
            });
        }

        return Ok();
    }

    [HttpPost("{id}/media")]
    public async Task<ActionResult> RegisterNewMediaForProductVariation(int id, [FromForm] AddNewMediaRequest newMediaRequest)
    {
        var result = await productVariationService.InsertNewMedia(id, mapper.Map<AddNewMediaDto>(newMediaRequest));

        if (!result)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Insertion failed",
                Detail = "Insertion failed with unknown reason"
            });
        }

        return Ok();
    }


    [HttpDelete("{variationId}/media/{mediaId}")]
    public async Task<ActionResult> DeleteVariationMedia(int variationId, int mediaId)
    {
        var result = await productVariationService.deleteVariationsMedia(variationId, mediaId);

        if (!result)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Deletion failed",
                Detail = "Deletion failed with unknown reason"
            });
        }

        return Ok();
    }
}

