using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HiTechStore.Data.DTOs.Component;

public class ComponentCreationDto
{
    [Required]
    [MinLength(2)]
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    [MinLength(10)]
    [JsonPropertyName("description")]
    public string? Description { get; set; }
    [Required]
    [MinLength(1)]
    [JsonPropertyName("properties")]
    public IEnumerable<PropertyEntryCreationDto>? Properties { get; set; }
}

public class ComponentCreationOrReferenceDto : ComponentCreationDto
{
    public int? ComponentId { get; set; }
}
