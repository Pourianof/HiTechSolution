namespace HiTechStore.Data.DTOs.Cart;

public class CartWithProductsDto
{
    public int CartId { get; set; }
    public DateTime CreatedAt { get; set; }
    public IEnumerable<CartItemWithProductDto>? Items { get; set; }
}

public class CartItemWithProductDto
{
    public MinimalProductDto? Product { get; set; }
    public int Amount { get; set; }
}