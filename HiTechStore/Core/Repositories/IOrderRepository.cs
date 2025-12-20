using HiTechStore.Data.DTOs.Order;
using HiTechStore.Models;

namespace HiTechStore.Core.Repositories;

public interface IOrderRepository : IRepository<Order>
{
    Task<Order?> GetUserPendingOrder(string userId);
    Task<IEnumerable<Order>?> GetUserPaidOrders(string userId);
    Task<IEnumerable<OrderWithProductsDto>?> GetUserOrders(string userId);

}