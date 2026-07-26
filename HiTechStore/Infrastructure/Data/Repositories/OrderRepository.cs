


using AutoMapper;

using HiTechStore.Core.Common.Interfaces.Infra.Repositories;
using HiTechStore.Core.Models;

using Microsoft.EntityFrameworkCore;
using HiTechStore.Infrastructure.Data.Queries;
using HiTechStore.Infrastructure.Data.DTOs;
using HiTechStore.Core.Dto.Order;
using HiTechStore.Helpers.URLFilterQuery;

namespace HiTechStore.Infrastructure.Data.Repositories;

public class OrderRepository : RepositoryWithIntegerId<Order, OrderDto>, IOrderRepository
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

    public async Task<PagedResultDto<OrderWithProductsDto>> GetUserOrders(string userId, BaseQuery query)
    {
        var orderQuery = _context.Orders.Where(
                    order => order.Client!.Id == userId
                );

        var sortBy = query.SortBy?.GetValue<string>(QueryOperator.Equal);

        if (!string.IsNullOrEmpty(sortBy))
        {
            orderQuery = sortBy switch
            {
                "placed_on" => orderQuery.OrderBy(o => o.CreatedAt),
                _ => orderQuery.OrderBy(o => o.CreatedAt),
            };
        }

        var sortDir = query.SortDir?.GetValue<string>(QueryOperator.Equal);

        return await GetPagedResult<OrderWithProductsDto>(
            orderQuery,
            query
        );
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