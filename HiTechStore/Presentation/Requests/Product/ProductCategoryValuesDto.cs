using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

using HiTechStore.Core.Dto.Product;
using HiTechStore.Helpers.AutoMapper;
using HiTechStore.Presentation.MapConverters;

namespace HiTechStore.Presentation.Product;

[MapTo<ProductCategoryValuesDto>]
public class ProductCategoryValuesRequest
{
    [Required]
    [JsonPropertyName("categoryId")]
    public int? CategoryId { get; set; } = null; // null just for model binding error
    [JsonPropertyName("properties")]
    public IEnumerable<PropertyValueEntryCreationRequest>? Properties { get; set; }
    [JsonPropertyName("componentModels")]
    public IEnumerable<int>? ComponentModels { get; set; }

}

[MapTo<PropertyValueEntryCreationDto>]
public class PropertyValueEntryCreationRequest
{
    [Required]
    [JsonPropertyName("propertyId")]
    public int? PropertyId { get; set; } = null;
    [Required]
    [JsonPropertyName("propertyValue")]
    [MapToProperty(
        targetPropertyName: nameof(PropertyValueEntryCreationDto.PropertyValue),
        converter: typeof(JsonElementToObjectConverter)
    )]
    public object? PropertyValue { get; set; }
}
