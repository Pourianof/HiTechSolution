
using AutoMapper;

using HiTechStore.Core.Repositories;
using HiTechStore.Data.DTOs.Order;
using HiTechStore.Models;

using Microsoft.EntityFrameworkCore;

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

    public async Task<IEnumerable<OrderWithProductsDto>?> GetUserOrders(string userId)
    {
        return await Project<OrderWithProductsDto>(_context.Orders.Where(
                    order => order.Client!.Id == userId
                )).ToListAsync();
    }
}