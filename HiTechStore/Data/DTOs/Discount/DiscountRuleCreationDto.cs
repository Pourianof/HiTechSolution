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
    [Required]
    [MinLength(3)]
    [MapUsing<ScriptToConditionComponentResolver>]
    [MapToProperty(nameof(DiscountRule.ConditionTree))]
    public string? Script { get; set; }
    [Required]
    public DiscountActionCreationDto? DiscountAction { get; set; }
}
