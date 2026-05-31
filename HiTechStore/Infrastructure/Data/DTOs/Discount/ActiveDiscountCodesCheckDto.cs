using System.ComponentModel.DataAnnotations;

using HiTechStore.Infrastructure.Data.DTOs.Validations;

namespace HiTechStore.Infrastructure.Data.DTOs.Discount;

public class ActiveDiscountCodesCheckDto
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