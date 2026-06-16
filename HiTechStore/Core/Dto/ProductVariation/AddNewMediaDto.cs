using HiTechStore.Core.Common.Interfaces.Infra;

namespace HiTechStore.Core.Dto.ProductVariation;

public class AddNewMediaDto
{
    public AppFile? File { get; set; }
    public AppFile? Thumbnail { get; set; }
}