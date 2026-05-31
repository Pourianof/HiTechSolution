using System.ComponentModel.DataAnnotations;

using HiTechStore.Infrastructure.Data.DTOs.Cart;

namespace HiTechStore.Infrastructure.Data.DTOs;

public class CartDto
{
    [Required]
    [MinLength(1)]
    public IEnumerable<CartItemDto>? Items { get; set; }
}