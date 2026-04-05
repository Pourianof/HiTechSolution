using System.ComponentModel.DataAnnotations;

namespace HiTechStore.Data.DTOs.Discount;

public class DiscountCodeUpdateDto
{
    public bool? IsDeactivated { get; set; }
    [MinLength(5)]
    public string? Description { get; set; }
}
