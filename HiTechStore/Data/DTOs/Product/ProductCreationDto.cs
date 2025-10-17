using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

using HiTechStore.Data.DTOs;
using HiTechStore.Data.DTOs.Binders;
using HiTechStore.Data.DTOs.Product;
using HiTechStore.Data.DTOs.Product.Validations;

namespace HiTechStore.DTOs.Product;

public class ProductCreationDto
{
    [Required]
    [MinLength(3)]
    [MaxLength(100)]
    public string? Title { get; set; }

    [Required]
    [Range(0, 10000000)]
    public decimal? Price { get; set; }
    public int? BrandModel { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }
    [ProductMediaValidation]
    public IEnumerable<IFormFile>? Media { get; set; }
    [Required]
    [FromJson]
    public ProductCategoryValuesDto? CategoryValues { get; set; }
    [FromJson]
    public MediaMetaDataDto? MediaMetaData { get; set; }
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

