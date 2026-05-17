using HiTechStore.Core.Dto.ProductVariation;
using HiTechStore.Data.DTOs.Validations;
using HiTechStore.Helpers.AutoMapper;

namespace HiTechStore.Presentation.Requests.ProductVariation;

[MapTo<UpdateProductVariationDetailsDto>]
public class UpdateVariationDetailsRequest
{
    [NonZeroPositiveNumber]
    public double? Price { get; set; }
    [PositiveNumber]
    public int? Inventory { get; set; }
    [NonZeroPositiveNumber]
    public int? ColorId { get; set; }
}