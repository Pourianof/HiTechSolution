using HiTechStore.Core.Dto.ProductVariation;
using HiTechStore.Infrastructure.Data.DTOs;
using HiTechStore.Infrastructure.Data.DTOs.Product;

namespace HiTechStore.Core.Services.ProductVariation;

public interface IProductVariationService
{
    Task<ProductVariationDto?> UpdateDetails(int variationId, UpdateProductVariationDetailsDto updateDto);
    Task<ProductMediaDto> InsertNewMedia(int variationId, AddNewMediaDto newMediaDto);
    Task<bool> deleteVariationsMedia(int variationId, int mediaId);
}