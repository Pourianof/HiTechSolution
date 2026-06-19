using System.ComponentModel.DataAnnotations;

using HiTechStore.Core.Dto.Cart;
using HiTechStore.Helpers.AutoMapper;
using HiTechStore.Infrastructure.Data.DTOs;

namespace HiTechStore.Presentation.Requests.Cart;

[MapTo<CartDto>]
public class CartRequest
{
    [Required]
    [MinLength(1)]
    public IEnumerable<CartItemDto>? Items { get; set; }
}