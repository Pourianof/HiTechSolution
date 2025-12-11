using System.ComponentModel.DataAnnotations;

namespace HiTechStore.Data.DTOs.Cart;

public class UpdateCartItemListDto
{
    [Required]
    [MinLength(1)]
    public IEnumerable<CartItemDto>? CartItems { get; set; }
}
