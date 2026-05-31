using System.Text.Json.Serialization;

using HiTechStore.Infrastructure.Data.DTOs.Product;
using HiTechStore.Helpers.AutoMapper;
using HiTechStore.Core.Models;

namespace HiTechStore.Infrastructure.Data.DTOs.Order;

public class OrderWithProductsDto
{
    public int OrderId { get; set; }
    public DateTime CreatedAt { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter<OrderPaymentState>))]
    public OrderPaymentState PaymentState { get; set; } = OrderPaymentState.Pending;
    public List<OrderItemWithProductDto>? Items { get; set; }
}

public class OrderItemWithProductDto
{
    public int Id { get; set; }
    public OrderItemProductVariationDto? ProductVariation { get; set; }
    public int Count { get; set; }
    public double OrderPayTimePrice { get; set; }
    public double? Discount { get; set; }
}

[MapFrom<ProductVariation>]
public class OrderItemProductVariationDto
{
    public int ProductVariationId { get; set; }
    public double Price { get; set; }
    public Color? Color { get; set; }
    public List<ProductMediaDto> Media { get; set; } = new();
    public ProductSummaryDto? Product { get; set; }
}

[MapFrom<Core.Models.Product>]
public class ProductSummaryDto
{
    public int ProductId { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
}