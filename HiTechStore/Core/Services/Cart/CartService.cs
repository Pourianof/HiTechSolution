
using HiTechStore.Core.Common.Interfaces.Infra;
using HiTechStore.Core.Common.Interfaces.Presentation;
using HiTechStore.Core.Dto.Cart;
using HiTechStore.Core.Dto.Discount;
using HiTechStore.Core.Helpers.Result;
using HiTechStore.Core.Models;
using HiTechStore.Core.Services.Authorization;
using HiTechStore.Infrastructure.Data.DTOs;

namespace HiTechStore.Core.Services.Cart;

public class CartService : ServiceBase, ICartService
{
    private IUnitOfWork _unitOfWork;
    private IPublicAssetRegisterer _assetRegisterer;
    public CartService(IUnitOfWork unitOfWork, IPublicAssetRegisterer assetRegisterer, IAuthorizationService authorizationService, ICurrentUserProvider currentUserProvider) : base(authorizationService, currentUserProvider)
    {
        _unitOfWork = unitOfWork;
        _assetRegisterer = assetRegisterer;
    }

    public async Task ResolveCartMediaPublicUrl(CartWithProductsDto? cart)
    {
        if (cart is null)
        {
            return;
        }

        foreach (var item in cart.Items?.SelectMany(i => i.Variations) ?? [])
        {
            foreach (var media in item.Variation?.Media ?? [])
            {
                if (media.Url is not null)
                {
                    media.Url = _assetRegisterer.GetPublicUrl(media.Url);
                }

                if (media.ThumbnailUrl is not null)
                {
                    media.Url = _assetRegisterer.GetPublicUrl(media.ThumbnailUrl);
                }
            }
        }
    }

    public async Task<Result<CartWithProductsDto>> GetUserCart()
    {
        var cart = await _unitOfWork.CartRepository.GetUserActiveCartWithProductAsync(UserIdOrThrow);

        await ResolveCartMediaPublicUrl(cart);

        return new()
        {
            Value = cart
        };

    }

    public async Task<Result<CartWithProductsDto>> SyncCart(CartDto cartDto)
    {
        var specifiedProductIds = cartDto.Items!.Select(i => i.ProductVariationId);
        var addingCartItemProducts = await _unitOfWork.Products.GetAllVariations(specifiedProductIds);

        if (addingCartItemProducts.Count() != specifiedProductIds.Count())
        {
            var notExistedProducts = specifiedProductIds.Select((p, i) => new { ProductId = p, Index = i }).Where(
                indexedProd => !addingCartItemProducts.Any(p => p.Product!.ProductId == indexedProd.ProductId)
            );

            var result = new Result<CartWithProductsDto>() { Errors = [] };
            foreach (var product in notExistedProducts)
            {
                result.Errors.Append(
                    CartErrors.NotFoundProduct(product.Index)
                );
            }

            if (result.HasError)
            {
                return result;
            }

            // Check requesed amount against available amounts
            for (int index = 0; index < addingCartItemProducts.Count(); index++)
            {
                var pv = addingCartItemProducts.ElementAt(index);
                var requestedAmount = cartDto.Items!.First(i => i.ProductVariationId == pv.ProductVariationId).Amount;

                if (pv.Inventory > requestedAmount)
                {
                    result.Errors.Append(
                        CartErrors.OutOfAmount(index, requestedAmount, pv.Inventory)
                    );
                }
            }

            if (result.HasError)
            {
                return result;
            }
        }

        var user = await GetUser();
        var cart = await _unitOfWork.CartRepository.GetUserActiveCartAsync(user.Id);


        // check if user has active cart or not
        if (cart is not null)
        {
            var newProductItems = cartDto.Items!
                .Where(item => !cart.Items.Any(i => i.ProductVariationId == item.ProductVariationId))
                .Select(
                    item => new CartItem()
                    {
                        Amount = item.Amount,
                        ProductVariationId = item.ProductVariationId
                    }
                );

            cart.Items = cart.Items.Select(
                item => new CartItem
                {
                    ProductVariationId = item.ProductVariationId,
                    Amount = cartDto.Items!.FirstOrDefault(i => i.ProductVariationId == item.ProductVariationId)?.Amount ?? item.Amount,
                }
            ).ToList();
            cart.Items.AddRange(newProductItems);

            // remove items with 0 count
            cart.Items = cart.Items.Where(item => item.Amount != 0).ToList();
        }
        else
        {
            cart = new Models.Cart()
            {
                ClientId = user.Id,
                Items = cartDto.Items!.Select(item => new CartItem()
                {
                    Amount = item.Amount,
                    ProductVariationId = item.ProductVariationId
                }).ToList()
            };
            // create total new cart
            await _unitOfWork.CartRepository.AddAsync(cart);
        }

        await _unitOfWork.Complete();

        var finalCart = (await _unitOfWork.CartRepository.GetUserActiveCartWithProductAsync(user.Id))!;

        await ResolveCartMediaPublicUrl(finalCart);

        return new()
        {
            Value = finalCart
        };
    }
}