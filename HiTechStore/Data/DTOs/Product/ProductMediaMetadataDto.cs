using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HiTechStore.Data.DTOs.Product;

public class MediaMetaDataDto
{
    [JsonPropertyName("isMain")]
    public bool IsMain { get; set; } = false;
    [Required]
    [JsonPropertyName("fileName")]
    public string? FileName { get; set; }
}