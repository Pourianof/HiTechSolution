using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

using HiTechStore.Infrastructure.Data.DTOs.JsonConverters;
using HiTechStore.Helpers.AutoMapper;

namespace HiTechStore.Infrastructure.Data.DTOs.Discount;


[MapTo<Core.Models.Discount>]
public class DiscountCreationDto
{
    [MinLength(5)]
    [Required]
    public string? Description { get; set; }
    [Required]
    [JsonConverter(typeof(UnixDateTimeConverter))]
    public DateTime StartTime { get; set; }
    [Required]
    [JsonConverter(typeof(UnixDateTimeConverter))]
    public DateTime EndTime { get; set; }
    [Required]
    [MinLength(1)]
    public ICollection<DiscountRuleCreationDto>? Rules { get; set; }
}



[MapTo<Core.Models.Discount>]
public class DiscountCodeCreationDto : DiscountCreationDto
{
    [Required]
    [MinLength(3)]
    [MaxLength(12)]
    public string? Code { get; set; }
}
