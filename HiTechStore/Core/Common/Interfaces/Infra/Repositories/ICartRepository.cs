using HiTechStore.Core.Dto.Cart;
using HiTechStore.Core.Models;

namespace HiTechStore.Core.Common.Interfaces.Infra.Repositories;

public interface ICartRepository : IRepository<Cart>
{
    Task<Cart?> GetUserActiveCartAsync(string userId);
    Task<CartWithProductsDto?> GetUserActiveCartWithProductAsync(string userId);

}