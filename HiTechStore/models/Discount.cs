using HiTechStore.Core;

namespace HiTechStore.Models;


public class Discount : IModel
{
    public int DiscountId { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? Code { get; set; }
    public bool IsDiscountCode { get; set; }
    public string? Description { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public bool IsDeactivated { get; set; }
    public string? CreatorId { get; set; }
    virtual public User? Creator { get; set; }
    virtual public ICollection<DiscountRule>? Rules { get; set; }
}
