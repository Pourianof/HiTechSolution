using HiTechStore.Core;

namespace HiTechStore.Models;


public class Cart : IModel
{
    public int CartId { get; set; }
    public virtual List<CartItem> Items { get; set; } = new List<CartItem>();
    public DateTime CreatedAt { get; set; }
    public virtual User? Client { get; set; }
    public string? ClientId { get; set; }
}

public class CartItem : IModel
{
    public int CartItemId { get; set; }
    public virtual Product? Product { get; set; }
    public int ProductId { get; set; }
    public virtual Cart? Cart { get; set; }
    public int Amount { get; set; }
}