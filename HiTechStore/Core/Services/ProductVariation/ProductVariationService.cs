using AutoMapper;

using HiTechStore.Core.Common.Interfaces.Presentation;
using HiTechStore.Core.Dto.ProductVariation;
using HiTechStore.Core.Exceptions;
using HiTechStore.Core.Services.Authorization;
using HiTechStore.Infrastructure.AssetStorage;
using HiTechStore.Infrastructure.Data.DTOs;
using HiTechStore.Infrastructure.Data.DTOs.Product;

namespace HiTechStore.Core.Services.ProductVariation;

public class ProductVariationService : ServiceBase, IProductVariationService
{
    private IUnitOfWork _unitOfWork;
    private ProductMediaRegisterer _mediaRegisterer;
    private IMapper _mapper;

    public ProductVariationService(
        IAuthorizationService authorizationService,
        ICurrentUserProvider currentUserProvider,
        IUnitOfWork unitOfWork,
        ProductMediaRegisterer mediaRegisterer,
        IMapper mapper
    ) : base(authorizationService, currentUserProvider)
    {
        _unitOfWork = unitOfWork;
        _mediaRegisterer = mediaRegisterer;
        _mapper = mapper;
    }

    private async Task<Core.Models.ProductVariation> GetVariation(int variationId)
    {
        var user = await GetUser();

        var productVariation = await _unitOfWork.ProductVariationRepository.GetModelByIdAsync(variationId);

        if (productVariation is null)
        {
            throw new NotFoundException($"product variation with id {variationId} has not found");
        }

        if (user.Id != productVariation.Product!.AuthorId)
        {
            throw new NotAllowedException("Not allowed", "You are not authorized to access or manipulate this variation");
        }

        return productVariation;
    }

    public async Task<ProductVariationDto?> UpdateDetails(int variationId, UpdateProductVariationDetailsDto updateDto)
    {
        var isColorSpecified = updateDto.ColorId is not null && updateDto.ColorId.Value > 0;
        var isPriceSpecified = updateDto.Price is not null && updateDto.Price.Value > 0;
        var isInventorySpecified = updateDto.Inventory is not null && updateDto.Inventory.Value > 0;

        if (!isColorSpecified && !isPriceSpecified && !isInventorySpecified)
        {
            return default;
        }

        var productVariation = await GetVariation(variationId);

        var isSameColor = !isColorSpecified || isColorSpecified && updateDto.ColorId!.Value == productVariation.Color!.ColorId;
        var isSameInventory = !isInventorySpecified || isInventorySpecified && updateDto.Inventory == productVariation.Inventory;
        var isSamePrice = !isPriceSpecified || isPriceSpecified && updateDto.Price == productVariation.Price;

        if (
            isSameColor &&
            isSameInventory &&
            isSamePrice
        )
        {
            return default;
        }

        if (!isSameColor)
        {
            var color = await _unitOfWork.ColorRepository.GetModelByIdAsync(updateDto.ColorId!.Value);

            if (color is null)
            {
                throw new NotFoundException($"color with id {updateDto.ColorId.Value} not found");
            }

            productVariation.Color = color;
        }

        if (!isSameInventory)
        {
            var newInventory = updateDto.Inventory!.Value;
            if (newInventory < 0)
            {
                throw new ModelException("Bad input", "product variation's price could not be negative", $"{nameof(UpdateProductVariationDetailsDto.Inventory)}");
            }
            productVariation.Inventory = newInventory;
        }

        if (!isSamePrice)
        {
            var newPrice = updateDto.Price!.Value;
            if (newPrice <= 0)
            {
                throw new ModelException("Bad input", "product variation's price could not be zero or negative", $"{nameof(UpdateProductVariationDetailsDto.Price)}");
            }
            productVariation.Price = newPrice;
        }

        await _unitOfWork.Complete();

        return await _unitOfWork.ProductVariationRepository.GetByIdProjectedAsync(variationId);
    }

    public async Task<ProductMediaDto> InsertNewMedia(int variationId, AddNewMediaDto newMediaDto)
    {
        if (newMediaDto.File is null)
        {
            throw new ModelException("Bad input", $"{nameof(AddNewMediaDto.File)} field is required", nameof(AddNewMediaDto.File));
        }

        var productVariation = await GetVariation(variationId);

        var media = await _mediaRegisterer.RegisterMedia(productVariation.ProductId, new()
        {
            File = newMediaDto.File,
            IsMain = false,
            Thumbnail = newMediaDto.Thumbnail
        });

        productVariation.Media.Add(media);

        await _unitOfWork.Complete();

        return _mapper.Map<ProductMediaDto>(media);
    }

    public async Task<bool> deleteVariationsMedia(int variationId, int mediaId)
    {
        var variation = await GetVariation(variationId);

        var media = variation.Media.FirstOrDefault(m => m.ProductMediaId == mediaId);

        if (media is null)
        {
            throw new NotFoundException($"media with id {mediaId} not exist for variation with id {variationId}");
        }

        variation.Media.Remove(media);

        await _unitOfWork.Complete();

        return true;
    }
}

