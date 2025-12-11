using HiTechStore.Models;

namespace HiTechStore.Core.Repositories;

public interface ICartRepository : IRepository<Cart>
{
    Task<Cart?> GetUserActiveCartAsync(string userId);

}