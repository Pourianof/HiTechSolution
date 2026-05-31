using System.ComponentModel.DataAnnotations;

using HiTechStore.Infrastructure.Data.DTOs.Validations;

namespace HiTechStore.Infrastructure.Data.DTOs.Cart;

public class CartItemDto
{
    [Required]
    public int ProductVariationId { get; set; }
    [PositiveNumber]
    public int Amount { get; set; } = 1;
}
