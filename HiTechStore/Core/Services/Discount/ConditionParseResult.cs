using HiTechStore.Data.DTOs.Product;

namespace HiTechStore.Core.Services.Discount;

public class ConditionParseResult
{
    public IEnumerable<ProductDto>? ResultedProducts { get; set; }
    public string? Message { get; set; }
    public bool Succeed { get; set; } = false;
}