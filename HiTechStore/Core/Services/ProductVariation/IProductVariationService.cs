using HiTechStore.Core.Dto.ProductVariation;
using HiTechStore.Data.DTOs;
using HiTechStore.Data.DTOs.Product;

namespace HiTechStore.Core.Services.ProductVariation;

public interface IProductVariationService
{
    Task<ProductVariationDto?> UpdateDetails(int variationId, UpdateProductVariationDetailsDto updateDto);
    Task<ProductMediaDto> InsertNewMedia(int variationId, AddNewMediaDto newMediaDto);
    Task<bool> deleteVariationsMedia(int variationId, int mediaId);
}