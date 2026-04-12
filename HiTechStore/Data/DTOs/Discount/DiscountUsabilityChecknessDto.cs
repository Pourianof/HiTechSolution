using System.ComponentModel.DataAnnotations;

using HiTechStore.Data.DTOs.Validations;

namespace HiTechStore.Data.DTOs.Discount;

public class DiscountUsabilityChecknessDto
{
    [Required]
    public string? DiscountCode { get; set; }
    [Required]
    [PositiveNumber]
    public long StartTime { get; set; }
    [Required]
    [PositiveNumber]
    public long EndTime { get; set; }

}