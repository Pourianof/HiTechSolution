using HiTechStore.Core;

namespace HiTechStore.Models;


public class OrderItem : IModel
{
    public int Id { get; set; }
    public virtual Order? Order { get; set; }
    public virtual Product? Product { get; set; }
    public int Count { get; set; }
    public int OrderPayTimePrice { get; set; }
    public int? Discount { get; set; }
}