using System.ComponentModel.DataAnnotations;

namespace HiTechStore.Presentation.Requests.Order;

public class CreateOrderRequest
{
    public string? PaymentCallbackUrl { get; set; }
}