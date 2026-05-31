namespace HiTechStore.Infrastructure.Data.DTOs.Product;

public class ProductMediaDto
{
    public int ProductMediaId { get; set; }
    public bool IsMain { get; set; }
    public string? Url { get; set; }
    public string? Type { get; set; }
    public string? ThumbnailUrl { get; set; }
}