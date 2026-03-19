using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HiTechStore.Data.DTOs.Product;

public class MediaMetaDataDto
{
    [JsonPropertyName("isMain")]
    public bool IsMain { get; set; } = false;
    [Required]
    [JsonPropertyName("index")]
    public int Index { get; set; }
}