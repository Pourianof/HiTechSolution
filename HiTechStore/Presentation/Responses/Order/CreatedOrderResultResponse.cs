using HiTechStore.Core.Dto.Order;

namespace HiTechStore.Infrastructure.Data.DTOs;

public class CreatedOrderResultResponse
{
    required public string PaymentCallbackUrl { get; set; }
    required public OrderWithProductsDto Order { get; set; }
}