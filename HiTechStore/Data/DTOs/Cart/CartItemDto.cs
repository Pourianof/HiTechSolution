using System.ComponentModel.DataAnnotations;

using HiTechStore.Data.DTOs.Validations;

namespace HiTechStore.Data.DTOs.Cart;

public class CartItemDto
{
    [Required]
    public int ProductId { get; set; }
    [PositiveNumber]
    public int Amount { get; set; } = 1;
}
