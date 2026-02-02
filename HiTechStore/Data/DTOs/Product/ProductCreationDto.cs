using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

using HiTechStore.Data.DTOs;
using HiTechStore.Data.DTOs.Binders;
using HiTechStore.Data.DTOs.Product;
using HiTechStore.Data.DTOs.Product.Validations;
using HiTechStore.Data.DTOs.Validations;

namespace HiTechStore.DTOs.Product;

public class ProductCreationDto
{
    [Required]
    [MinLength(3)]
    [MaxLength(100)]
    public string? Title { get; set; }
    public int? BrandModel { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    [Required]
    [FromJson]
    public ProductCategoryValuesDto? CategoryValues { get; set; }

    [Required]
    [ProductVariationValidation]
    [FromJson]
    [MinLength(1)]
    public IEnumerable<ProductVariationCreationDto>? Variations { get; set; }
    [ProductMediaValidation]
    public IEnumerable<IFormFile>? Media { get; set; }
}

public class ProductVariationCreationDto
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
    public IEnumerable<MediaMetaDataDto>? MediaMetaData { get; set; }
}


public class ProductCategoryValuesDto
{
    [Required]
    [JsonPropertyName("categoryId")]
    public int? CategoryId { get; set; } = null; // null just for model binding error
    [JsonPropertyName("properties")]
    public IEnumerable<PropertyValueEntryCreationDto>? Properties { get; set; }
    [JsonPropertyName("componentModels")]
    public IEnumerable<int>? ComponentModels { get; set; }

}

