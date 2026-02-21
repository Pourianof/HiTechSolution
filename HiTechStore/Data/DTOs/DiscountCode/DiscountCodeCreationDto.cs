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
    [Required]
    [MinLength(1)]
    public List<DiscountConditionGroupCreationDto> Conditions { get; set; } = new();
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

[MapTo<DiscountConditionGroup>]
public class DiscountConditionGroupCreationDto
{
    [Required]
    [MinLength(1)]
    public ICollection<DiscountConditionCreationDto>? Conditions { get; set; }
}

[MapTo<DiscountCondition>]
public class DiscountConditionCreationDto
{
    [Required]
    public int? EntityPropertyId { get; set; }
    [PositiveNumber]
    public int? Priority { get; set; }
    [Required]
    public DiscountOperation? Operation { get; set; }
    [Required]
    public string? Value { get; set; }
}