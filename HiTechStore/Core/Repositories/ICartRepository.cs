using HiTechStore.Data.DTOs.Cart;
using HiTechStore.Models;

namespace HiTechStore.Core.Repositories;

public interface ICartRepository : IRepository<Cart>
{
    Task<Cart?> GetUserActiveCartAsync(string userId);
    Task<CartWithProductsDto?> GetUserActiveCartWithProductAsync(string userId);

}