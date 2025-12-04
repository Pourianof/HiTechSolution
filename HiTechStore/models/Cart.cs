using HiTechStore.Core;

namespace HiTechStore.Models;


public class Cart : IModel
{
    public List<Product> Products { get; set; } = new List<Product>();
    public DateTime CreatedAt { get; set; }
}