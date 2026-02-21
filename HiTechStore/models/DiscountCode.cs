using HiTechStore.Core;

namespace HiTechStore.Models;


public class DiscountCode : IModel
{
    public int DiscountCodeId { get; set; }
    public string? Code { get; set; }

    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }

    virtual public ICollection<DiscountRule>? Rules { get; set; }
}