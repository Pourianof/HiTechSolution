using HiTechStore.Core;

namespace HiTechStore.Models;

public enum OrderPaymentState
{
    Paid,
    Pending
}

public class Order : IModel
{
    public int OrderId { get; set; }
    public DateTime CreatedAt { get; set; }
    public OrderPaymentState PaymentState { get; set; } = OrderPaymentState.Pending;
    public virtual List<OrderItem>? Items { get; set; }
    public virtual User? Client { get; set; }
    public virtual string? ClientId { get; set; }
}