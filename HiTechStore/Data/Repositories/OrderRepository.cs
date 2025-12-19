
using AutoMapper;

using HiTechStore.Core.Repositories;
using HiTechStore.Models;

namespace HiTechStore.Data.Repositories;

public class OrderRepository : Repository<Order>, IOrderRepository
{
    public OrderRepository(HiTechStoreDbContext context, IMapper mapper) : base(context, mapper)
    { }

    public async Task<Order?> GetUserPendingOrder(string userId)
    {
        return _context.Orders.Where(
            order => order.Client!.Id == userId
                && order.PaymentState != OrderPaymentState.Pending
        ).FirstOrDefault();
    }

    public async Task<IEnumerable<Order>?> GetUserPaidOrders(string userId)
    {
        return _context.Orders.Where(
                    order => order.Client!.Id == userId
                        && order.PaymentState != OrderPaymentState.Paid
                ).ToList();
    }
}