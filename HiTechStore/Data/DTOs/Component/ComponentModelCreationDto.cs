using System.ComponentModel.DataAnnotations;

namespace HiTechStore.Data.DTOs.Component;

public class ComponentModelCreationDto
{
    public int? BrandModelId { get; set; }
    public string? Description { get; set; }
    [Required]
    [MinLength(1)]
    public IEnumerable<PropertyValueEntryCreationDto>? Properties { get; set; }
}