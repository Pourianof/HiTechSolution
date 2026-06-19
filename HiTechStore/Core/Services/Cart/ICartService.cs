using HiTechStore.Core.Dto.Cart;
using HiTechStore.Core.Helpers.Result;
using HiTechStore.Infrastructure.Data.DTOs;

namespace HiTechStore.Core.Services.Cart;

public interface ICartService
{
    Task<Result<CartWithProductsDto>> SyncCart(CartDto cartDto);
    Task<Result<CartWithProductsDto>> GetUserCart();
}