
using AutoMapper;
using AutoMapper.QueryableExtensions;

using HiTechStore.Core.Common.Interfaces.Infra.Repositories;
using HiTechStore.Infrastructure.Data.DTOs;
using HiTechStore.Infrastructure.Data.DTOs.Cart;
using HiTechStore.Infrastructure.Data.DTOs.Product;
using HiTechStore.Core.Models;

using Microsoft.EntityFrameworkCore;

namespace HiTechStore.Infrastructure.Data.Repositories;

public class CartRepository : Repository<Cart>, ICartRepository
{
    public CartRepository(HiTechStoreDbContext context, IMapper mapper) : base(context, mapper)
    {
    }

    public async Task<Cart?> GetUserActiveCartAsync(string userId)
    {
        return await _dbSet.Where(c => c.Client!.Id == userId).FirstOrDefaultAsync();
    }

    public async Task<CartWithProductsDto?> GetUserActiveCartWithProductAsync(string userId)
    {
        return await _dbSet.Where(c => c.Client!.Id == userId).Select(
            cart => new CartWithProductsDto()
            {
                CartId = cart.CartId,
                CreatedAt = cart.CreatedAt,
                Items = cart.Items.GroupBy(item => item.ProductVariation!.Product).Select(
                    g => new MinimalProductDto()
                    {
                        Title = g.Key!.Title,
                        Description = g.Key.Description,
                        ProductId = g.Key.ProductId,
                        Variations = g.Select(
                            cartItem => new ProductVariationWithCartAmount()
                            {
                                Amount = cartItem.Amount,
                                Variation = new ProductVariationDto()
                                {
                                    ProductVariationId = cartItem.ProductVariationId,
                                    Color = cartItem.ProductVariation!.Color,
                                    Inventory = cartItem.ProductVariation.Inventory,
                                    Price = cartItem.ProductVariation.Price,
                                    Media = cartItem.ProductVariation.Media.Select(
                                   m => new ProductMediaDto()
                                   {
                                       IsMain = m.IsMain,
                                       ProductMediaId = m.ProductMediaId,
                                       Url = m.FilePath,
                                       Type = m.Type == MediaType.Image ? "Image" : "Video"
                                   }).ToList()
                                }
                            }
                        ),
                    }
                )
            }
        ).FirstOrDefaultAsync();
    }
}