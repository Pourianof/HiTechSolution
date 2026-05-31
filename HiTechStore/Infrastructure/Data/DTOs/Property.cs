using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

using HiTechStore.Core.Models;

namespace HiTechStore.Infrastructure.Data.DTOs;

public class PropertyDto
{
    public int PropertyId { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public PropertyType? PropertyType { get; set; }

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
    public PropertyType? PropertyType { get; set; } = Core.Models.PropertyType.String;

}

public class PropertyValueDto
{
    public object? Value { get; set; }
    public string? Name { get; set; }
    public int PropertyId { get; set; }
    public PropertyType ValueType { get; set; }
}

