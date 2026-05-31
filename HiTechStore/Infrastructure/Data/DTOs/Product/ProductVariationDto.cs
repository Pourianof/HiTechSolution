using HiTechStore.Infrastructure.Data.DTOs.Product;
using HiTechStore.Core.Models;

namespace HiTechStore.Infrastructure.Data.DTOs;

public class ProductVariationDto
{
    public int ProductVariationId { get; set; }
    public double Price { get; set; }
    public double Discount { get; set; } = 0.0;
    public Color? Color { get; set; }
    public int Inventory { get; set; }
    public virtual List<ProductMediaDto> Media { get; set; } = new();
}