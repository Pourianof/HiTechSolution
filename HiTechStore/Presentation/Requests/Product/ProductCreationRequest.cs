using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

using HiTechStore.Data.DTOs.Binders;
using HiTechStore.Data.DTOs.Product.Validations;
using HiTechStore.Data.DTOs.Validations;
using HiTechStore.Helpers.AutoMapper;
using HiTechStore.Presentation.Product;

namespace HiTechStore.Presentation.Requests;


[MapTo<ProductCreationDto>]
public class ProductCreationRequest
{
    [Required]
    [MinLength(3)]
    [MaxLength(100)]
    public string? Title { get; set; }
    [Required]
    public int? BrandModel { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    [Required]
    [FromJson]
    public ProductCategoryValuesRequest? CategoryValues { get; set; }

    [Required]
    [ProductVariationValidation]
    [FromJson]
    [MinLength(1)]
    public IEnumerable<ProductVariationCreationDto>? Variations { get; set; }
    [ProductMediaValidation]
    public IEnumerable<IFormFile>? Media { get; set; }
    public IEnumerable<IFormFile>? Thumbnails { get; set; }
}

[MapTo<ProductVariationCreationDto>]
public class ProductVariationCreationRequest
{
    [Required]
    [PositiveNumber]
    [JsonPropertyName("price")]
    public double Price { get; set; }
    [Required]
    [JsonPropertyName("color")]
    public int Color { get; set; }
    [PositiveNumber]
    [JsonPropertyName("inventory")]
    [Required]
    public int Inventory { get; set; }
    [Required]
    [MinLength(1)]
    [JsonPropertyName("mediaMetaData")]
    public IEnumerable<MediaMetaDataRequest>? MediaMetaData { get; set; }
}

[MapTo<MediaMetaDataDto>]
public class MediaMetaDataRequest
{
    [JsonPropertyName("isMain")]
    public bool IsMain { get; set; } = false;
    [Required]
    [JsonPropertyName("index")]
    public int Index { get; set; }
    [JsonPropertyName("thumbnailIndex")]
    public int? ThumbnailIndex { get; set; }
}