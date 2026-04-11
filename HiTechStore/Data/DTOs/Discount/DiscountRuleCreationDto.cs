using System.ComponentModel.DataAnnotations;

using HiTechStore.Data.Mapping.Discount;
using HiTechStore.Helpers.AutoMapper;
using HiTechStore.Models;

namespace HiTechStore.Data.DTOs.Discount;


[MapTo<DiscountRule>]
public class DiscountRuleCreationDto
{
    [Required]
    [MinLength(3)]
    public string? Name { get; set; }
    public string? Description { get; set; }
    [MinLength(3)]
    [MapToProperty(nameof(DiscountRule.ProductConditionTree), Converter = typeof(ScriptToConditionComponentResolver))]
    [MapToProperty(nameof(DiscountRule.ProductRawConditionScript))]
    public string? ProductScript { get; set; }
    [MinLength(3)]
    [MapToProperty(nameof(DiscountRule.UserConditionTree), Converter = typeof(ScriptToConditionComponentResolver))]
    [MapToProperty(nameof(DiscountRule.UserRawConditionScript))]
    public string? UserScript { get; set; }
    [Required]
    public DiscountActionCreationDto? DiscountAction { get; set; }
}
