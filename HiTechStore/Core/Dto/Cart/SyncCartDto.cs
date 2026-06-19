using HiTechStore.Core.Dto.Cart;

namespace HiTechStore.Infrastructure.Data.DTOs;

public class CartDto
{
    public IEnumerable<CartItemDto>? Items { get; set; }
}