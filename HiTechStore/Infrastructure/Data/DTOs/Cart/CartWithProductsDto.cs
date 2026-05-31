namespace HiTechStore.Infrastructure.Data.DTOs.Cart;

public class CartWithProductsDto
{
    public int CartId { get; set; }
    public DateTime CreatedAt { get; set; }
    public IEnumerable<MinimalProductDto>? Items { get; set; }
}
