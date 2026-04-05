using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

using HiTechStore.Data.DTOs.JsonConverters;
using HiTechStore.Helpers.AutoMapper;

namespace HiTechStore.Data.DTOs.Discount;

[MapTo<Models.Discount>]
public class DiscountCodeCreationDto
{
    [Required]
    [MinLength(3)]
    [MaxLength(12)]
    public string? Code { get; set; }
    [MinLength(5)]
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
