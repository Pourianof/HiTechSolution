using System.ComponentModel.DataAnnotations;

using HiTechStore.Data.DTOs.Cart;

namespace HiTechStore.Data.DTOs;

public class CartDto
{
    [Required]
    [MinLength(1)]
    public IEnumerable<CartItemDto>? Items { get; set; }
}