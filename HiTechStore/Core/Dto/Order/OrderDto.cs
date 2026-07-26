using HiTechStore.Core.Models;
using HiTechStore.Helpers.AutoMapper;

namespace HiTechStore.Core.Dto.Order;

[MapFrom<Models.Order>]
public class OrderDto
{
    public int OrderId { get; set; }
    public DateTime CreatedAt { get; set; }
    public OrderPaymentState PaymentState { get; set; } = OrderPaymentState.Pending;
    public string? ClientId { get; set; }
    public Models.Discount? DiscountCode { get; set; }
}