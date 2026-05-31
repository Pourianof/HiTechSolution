using HiTechStore.Infrastructure.Data.DTOs.Order;
using HiTechStore.Core.Models;

namespace HiTechStore.Core.Common.Interfaces.Infra.Repositories;

public interface IOrderRepository : IRepository<Order>
{
    Task<IEnumerable<Order>?> GetPendingOrders(DateTime? before = default);
    Task<Order?> GetUserPendingOrder(string userId);
    Task<IEnumerable<Order>?> GetUserPaidOrders(string userId);
    Task<IEnumerable<OrderWithProductsDto>?> GetUserOrders(string userId);

}