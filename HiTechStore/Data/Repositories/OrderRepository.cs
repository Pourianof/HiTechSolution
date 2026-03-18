

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
        return await _context.Orders.Where(
            order => order.Client!.Id == userId
                && order.PaymentState != OrderPaymentState.Pending
        ).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Order>?> GetUserPaidOrders(string userId)
    {
        return await _context.Orders.Where(
                    order => order.Client!.Id == userId
                        && order.PaymentState != OrderPaymentState.Paid
                ).ToListAsync();
    }

    public async Task<IEnumerable<OrderWithProductsDto>?> GetUserOrders(string userId)
    {
        return await Project<OrderWithProductsDto>(_context.Orders.Where(
                    order => order.Client!.Id == userId
                )).ToListAsync();
    }

    public async Task<IEnumerable<Order>?> GetPendingOrders(DateTime? before = null)
    {
        var query = _dbSet.Where((order) => order.PaymentState == OrderPaymentState.Pending);

        if (before is not null)
        {
            query = query.Where((order) => order.CreatedAt < before);
        }

        query = query.Include(order => order.Items)!.ThenInclude(item => item.ProductVariation);

        return await query.ToListAsync();
    }
}