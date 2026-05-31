using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HiTechStore.Infrastructure.Data.DTOs.Product;

public class MediaMetaDataDto
{
    [JsonPropertyName("isMain")]
    public bool IsMain { get; set; } = false;
    [Required]
    [JsonPropertyName("index")]
    public int Index { get; set; }
    [JsonPropertyName("thumbnailIndex")]
    public int? ThumbnailIndex { get; set; }
}