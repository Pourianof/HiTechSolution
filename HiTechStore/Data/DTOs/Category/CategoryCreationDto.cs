using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

using HiTechStore.Core.Validators;
using HiTechStore.Data.DTOs;
using HiTechStore.Data.DTOs.Binders;
using HiTechStore.Data.DTOs.Component;

namespace HiTechStore.DTOs.Category
{
    public class CategoryCreationDto
    {
        [Required]
        [MinLength(2)]
        [MaxLength(100)]
        public string? Name { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }
        [Required]
        public IFormFile? Image { get; set; }
        [FromJson]
        public IEnumerable<PropertyEntryCreationDto>? Properties { get; set; }
        [FromJson]
        public IEnumerable<ComponentCreationOrReferenceDto>? Components { get; set; }
        [Required]
        [ValidExtensions(["svg"])]
        public IFormFile? Icon { get; set; }
    }

}

public class ComponentCreationOrReferenceDto : IValidatableObject
{
    [JsonPropertyName("componentTypeId")]
    public int? ComponentTypeId { get; set; }
    [JsonPropertyName("newComponent")]
    public ComponentCreationDto? NewComponent { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (ComponentTypeId is null && NewComponent is null)
        {
            yield return new ValidationResult($"Must refer to an existing component by setting '{nameof(ComponentTypeId)}' or create new one in 'newComponent' field");
        }
    }
}