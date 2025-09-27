using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

using HiTechStore.Models;

namespace HiTechStore.Data.DTOs;

public class PropertyDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public DataType? PropertyType { get; set; }

}

public class PropertyEntryCreationDto
{
    [Required]
    [MinLength(2)]
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    [Required]
    [MinLength(10)]
    [JsonPropertyName("description")]
    public string? Description { get; set; }
    [MinLength(2)]
    [JsonPropertyName("unit")]
    public string? Unit { get; set; }
    [JsonPropertyName("propertyType")]
    public PropertyType? PropertyType { get; set; } = Models.PropertyType.String;

}

public class PropertyValueDto
{
    public string? Value { get; set; }
    public string? Name { get; set; }
    public int PropertyId { get; set; }
    public PropertyType ValueType { get; set; }
}