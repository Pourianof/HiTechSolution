using System.ComponentModel.DataAnnotations;

using HiTechStore.Core.Dto.Cart;
using HiTechStore.Helpers.AutoMapper;
using HiTechStore.Infrastructure.Data.DTOs.Validations;

namespace HiTechStore.Presentation.Requests.Cart;

[MapTo<CartItemDto>]
public class CartItemRequest
{
    [Required]
    public int ProductVariationId { get; set; }
    [PositiveNumber]
    public int Amount { get; set; } = 1;
}
