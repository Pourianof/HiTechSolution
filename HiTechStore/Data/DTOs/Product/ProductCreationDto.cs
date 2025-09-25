using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

using HiTechStore.Data.DTOs.Binders;
using HiTechStore.Data.DTOs.Product;
using HiTechStore.Data.DTOs.Product.Validations;
using HiTechStore.Helpers.Types;

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
        public ProductCategoryValuesDto? CategoryValues { get; set; }
        [FromJson]
        public MediaMetaDataDto? MediaMetaData { get; set; }
    }
}

public class ComponentsPropertiesValuesDto
{
    [Required]
    [JsonPropertyName("componentModelId")]
    public int? ComponentModelId { get; set; } = null; // null just for model binding error
    [Required]
    [MinLength(1)]
    [JsonPropertyName("properties")]
    public IEnumerable<PropertyValueEntryDto>? Properties { get; set; }
}


public class ProductCategoryValuesDto
{
    [Required]
    [JsonPropertyName("categoryId")]
    public int? CategoryId { get; set; } = null; // null just for model binding error
    [JsonPropertyName("properties")]
    public IEnumerable<PropertyValueEntryDto>? Properties { get; set; }
    [JsonPropertyName("componentsValues")]
    public ComponentsPropertiesValuesDto? ComponentsPropertiesValues { get; set; }

}

public class PropertyValueEntryDto
{
    [Required]
    [JsonPropertyName("propertyId")]
    public int? PropertyId { get; set; } = null;
    [Required]
    [JsonPropertyName("propertyValue")]
    public string? PropertyValue { get; set; }
}

