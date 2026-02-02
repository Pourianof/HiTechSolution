using HiTechStore.Data.DTOs.Product;
using HiTechStore.Models;

namespace HiTechStore.Data.DTOs;

public class ProductVariationDto
{
    public double Price { get; set; }
    public Color? Color { get; set; }
    public int Inventory { get; set; }
    public virtual List<ProductMediaDto> Media { get; set; } = new();
}