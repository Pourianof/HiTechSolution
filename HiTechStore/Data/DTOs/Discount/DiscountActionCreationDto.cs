using System.ComponentModel.DataAnnotations;

using HiTechStore.Data.DTOs.Validations;
using HiTechStore.Helpers.AutoMapper;
using HiTechStore.Models;



namespace HiTechStore.Data.DTOs.Discount;

[MapTo<DiscountAction>]
public class DiscountActionCreationDto
{
    [Required]
    public DiscountActionType Type { get; set; }
    [Required]
    [PositiveNumber]
    public decimal Value { get; set; }
}