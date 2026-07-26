using HiTechStore.Core.Models;
using HiTechStore.Infrastructure.Data.DTOs;
using HiTechStore.Infrastructure.Data.Queries;
using HiTechStore.Core.Dto.Order;

namespace HiTechStore.Core.Common.Interfaces.Infra.Repositories;

public interface IOrderRepository : IRepositoryWithIntegerId<Order, OrderDto>
{
    Task<IEnumerable<Order>?> GetPendingOrders(DateTime? before = default);
    Task<Order?> GetUserPendingOrder(string userId);
    Task<IEnumerable<Order>?> GetUserPaidOrders(string userId);
    Task<PagedResultDto<OrderWithProductsDto>> GetUserOrders(string userId, BaseQuery query);

}