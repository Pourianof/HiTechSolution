using HiTechStore.Core.Dto.ProductVariation;

namespace HiTechStore.Core.Services.ProductVariation;

public interface IProductVariationService
{
    Task<bool> UpdateDetails(int variationId, UpdateProductVariationDetailsDto updateDto);
    Task<bool> InsertNewMedia(int variationId, AddNewMediaDto newMediaDto);
    Task<bool> deleteVariationsMedia(int variationId, int mediaId);
}