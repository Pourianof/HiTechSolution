using HiTechStore.Core;

namespace HiTechStore.Models;

public enum MediaType
{
    Image, Video
}

public class ProductMedia : IModel
{
    public int ProductMediaId { get; set; }
    public bool IsMain { get; set; }
    public string? FilePath { get; set; }
    public MediaType Type { get; set; }
    public int ProductId { get; set; }
    public virtual Product? Product { get; set; }
}