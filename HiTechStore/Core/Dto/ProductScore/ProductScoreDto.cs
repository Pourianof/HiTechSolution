using HiTechStore.Helpers.AutoMapper;

namespace HiTechStore.Core.Dto.ProductScore;


[MapFrom<Models.ProductScore>]
public class ProductScoreDto
{
    [MapFromProperty(nameof(Models.ProductScore.Score))]
    public int Rate { get; set; }
}