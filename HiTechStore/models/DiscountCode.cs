using HiTechStore.Core;

namespace HiTechStore.Models;


public class DiscountCode : IModel
{
    public int DiscountCodeId { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? Code { get; set; }
    public string? Description { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public bool IsDeactivated { get; set; }
    public string? CreatorId { get; set; }
    virtual public User? Creator { get; set; }

    virtual public ICollection<DiscountRule>? Rules { get; set; }
}
