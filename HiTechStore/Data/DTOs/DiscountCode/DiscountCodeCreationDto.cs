using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

using HiTechStore.Data.DTOs.JsonConverters;
using HiTechStore.Data.DTOs.Validations;
using HiTechStore.Helpers.AutoMapper;
using HiTechStore.Models;

namespace HiTechStore.Data.DTOs.DiscountCode;

[MapTo<Models.DiscountCode>]
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

[MapTo<DiscountRule>]
public class DiscountRuleCreationDto
{
    [Required]
    [MinLength(3)]
    public string? Name { get; set; }
    public string? Description { get; set; }
    // [Required]
    // [MinLength(1)]
    // public List<DiscountConditionGroupCreationDto> Conditions { get; set; } = new();
    [Required]
    public DiscountActionCreationDto? DiscountAction { get; set; }
}

[MapTo<DiscountAction>]
public class DiscountActionCreationDto
{
    [Required]
    public DiscountActionType Type { get; set; }
    [Required]
    [PositiveNumber]
    public decimal Value { get; set; }
}

