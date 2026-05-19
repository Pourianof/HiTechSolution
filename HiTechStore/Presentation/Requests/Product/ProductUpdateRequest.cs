using HiTechStore.Core.Services.Product;
using HiTechStore.Helpers.AutoMapper;

namespace HiTechStore.Presentation.Product;

[MapTo<UpdateProductDto>]
public class ProductUpdateRequest
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public int? BrandModelId { get; set; }
}