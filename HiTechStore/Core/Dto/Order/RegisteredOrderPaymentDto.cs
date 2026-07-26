namespace HiTechStore.Core.Dto.Order;

public class RegisteredOrderPaymentDto
{
    required public OrderWithProductsDto Order { get; set; }
    required public string PaymentUrl { get; set; }
}