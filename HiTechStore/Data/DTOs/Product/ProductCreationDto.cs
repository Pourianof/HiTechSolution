using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

using HiTechStore.Data.DTOs;
using HiTechStore.Data.DTOs.Binders;
using HiTechStore.Data.DTOs.Component;
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
    public IEnumerable<ProductComponentModelCreationDto>? ComponentModels { get; set; }

}

public class ProductComponentModelCreationDto : IValidatableObject
{
    public ComponentModelCreationDto? Component { get; set; }
    public int? ComponentModelId { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Component is null && ComponentModelId is null)
        {
            yield return new ValidationResult($"You must refer to an existing component-model by its id({nameof(ComponentModelId)}) or define a new one({nameof(Component)})");
        }
        yield return ValidationResult.Success!;
    }
}