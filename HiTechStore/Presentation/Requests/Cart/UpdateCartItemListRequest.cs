using System.ComponentModel.DataAnnotations;

namespace HiTechStore.Presentation.Requests.Cart;

public class UpdateCartItemListRequest
{
    [Required]
    [MinLength(1)]
    public IEnumerable<CartItemRequest>? CartItems { get; set; }
}
