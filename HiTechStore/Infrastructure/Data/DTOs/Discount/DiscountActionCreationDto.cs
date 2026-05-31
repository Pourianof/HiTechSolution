using System.ComponentModel.DataAnnotations;

using HiTechStore.Infrastructure.Data.DTOs.Validations;
using HiTechStore.Helpers.AutoMapper;
using HiTechStore.Core.Models;



namespace HiTechStore.Infrastructure.Data.DTOs.Discount;

[MapTo<DiscountAction>]
public class DiscountActionCreationDto
{
    [Required]
    public DiscountActionType Type { get; set; }
    [Required]
    [PositiveNumber]
    public decimal Value { get; set; }
}