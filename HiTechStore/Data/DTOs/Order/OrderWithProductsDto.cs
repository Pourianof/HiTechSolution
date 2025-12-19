using System.Text.Json.Serialization;

using HiTechStore.Models;

namespace HiTechStore.Data.DTOs.Order;

public class OrderWithProductsDto
{
    public int OrderId { get; set; }
    public DateTime CreatedAt { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter<OrderPaymentState>))]
    public OrderPaymentState PaymentState { get; set; } = OrderPaymentState.Pending;
    public virtual List<OrderItemWithProductDto>? Items { get; set; }
}

public class OrderItemWithProductDto
{
    public int Id { get; set; }
    public virtual MinimalProductDto? Product { get; set; }
    public int Count { get; set; }
    public double OrderPayTimePrice { get; set; }
    public int? Discount { get; set; }
}