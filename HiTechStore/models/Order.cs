using HiTechStore.Core;

namespace HiTechStore.Models;

public enum OrderPaymentState
{
    Paid,
    Pending
}

public class Order : IModel
{
    public int OrderId { get; set; }
    public DateTime CreatedAt { get; set; }

}