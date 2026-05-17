using HiTechStore.Core.Dto.ProductVariation;
using HiTechStore.Helpers.AutoMapper;

namespace HiTechStore.Presentation.Requests.ProductVariation;

[MapTo<AddNewMediaDto>]
public class AddNewMediaRequest
{
    public IFormFile? File { get; set; }
    public IFormFile? Thumbnail { get; set; }
}