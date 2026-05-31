using AutoMapper;

using HiTechStore.Core.Dto.ProductVariation;
using HiTechStore.Core.Services.ProductVariation;
using HiTechStore.Infrastructure.Data.DTOs;
using HiTechStore.Infrastructure.Data.DTOs.Product;
using HiTechStore.Presentation.Requests.ProductVariation;

using Microsoft.AspNetCore.Mvc;

namespace HiTechStore.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductVariationsController(
    IProductVariationService productVariationService,
    IMapper mapper
) : ControllerBase
{
    [HttpPatch("{id}")]
    public async Task<ActionResult<ProductVariationDto>> UpdateVariationDetails(int id, UpdateVariationDetailsRequest updateRequest)
    {
        var result = await productVariationService.UpdateDetails(id, mapper.Map<UpdateProductVariationDetailsDto>(updateRequest));

        if (result is null)
        {
            // no changes
            return Ok();
        }

        return Ok(result);
    }

    [HttpPost("{id}/media")]
    public async Task<ActionResult<ProductMediaDto>> RegisterNewMediaForProductVariation(int id, [FromForm] AddNewMediaRequest newMediaRequest)
    {
        var result = await productVariationService.InsertNewMedia(id, mapper.Map<AddNewMediaDto>(newMediaRequest));

        if (result is null)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Insertion failed",
                Detail = "Insertion failed with unknown reason"
            });
        }

        return Ok(result);
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

