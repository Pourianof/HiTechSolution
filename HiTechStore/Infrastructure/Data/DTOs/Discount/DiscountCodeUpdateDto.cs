using System.ComponentModel.DataAnnotations;

namespace HiTechStore.Infrastructure.Data.DTOs.Discount;

public class DiscountCodeUpdateDto
{
    public bool? IsDeactivated { get; set; }
    [MinLength(5)]
    public string? Description { get; set; }
}
