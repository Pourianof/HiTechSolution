using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

using HiTechStore.Data.DTOs.Binders;
using HiTechStore.Data.DTOs.Product;
using HiTechStore.Data.DTOs.Product.Validations;

namespace HiTechStore.DTOs.Product
{
    public class ProductCreationDto
    {
        [Required]
        [MinLength(3)]
        [MaxLength(100)]
        public string? Title { get; set; }

        [Required]
        [Range(0, 10000000)]
        public decimal? Price { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }
        [ProductMediaValidation]
        public IEnumerable<IFormFile>? Media { get; set; }
        [Required]
        [FromJson]
        public ProductCategoryPropertyValuesDto? PropertiesValues { get; set; }
        public MediaMetaDataDto? MediaMetaData;
    }
}

public class ProductCategoryPropertyValuesDto
{
    [Required]
    [JsonPropertyName("categoryId")]
    public int? CategoryId { get; set; } = null; // null just for model binding error
    [Required]
    [MinLength(1)]
    [JsonPropertyName("properties")]
    public IEnumerable<ProductPropertyValueEntryDto>? Properties { get; set; }
}

public class ProductPropertyValueEntryDto
{
    [Required]
    [JsonPropertyName("propertyId")]
    public int? PropertyId { get; set; } = null;
    [Required]
    [JsonPropertyName("propertyValue")]
    public string? PropertyValue { get; set; }
}

