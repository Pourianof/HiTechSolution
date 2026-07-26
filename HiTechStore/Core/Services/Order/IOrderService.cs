using HiTechStore.Core.Dto.Order;
using HiTechStore.Core.Helpers.Result;
using HiTechStore.Infrastructure.Data.DTOs;
using HiTechStore.Infrastructure.Data.Queries;

namespace HiTechStore.Core.Services.Order;

public interface IOrderService
{
    Task<Result<RegisteredOrderPaymentDto>> CreateOrder(
        PaymentUrlFactoryDelegate paymentUrlFactoryDelegate,
        string? discountCode = default
    );
    Task<PagedResultDto<OrderWithProductsDto>> GetOrders(BaseQuery query);
    Task<OrderDto?> GetOrderById(int orderId);
    Task<Result<OrderWithProductsDto>> HandleOrderPayment(string orderConfirmationKey, string signedConfirmation);
}

public delegate string PaymentUrlFactoryDelegate(int orderId);

