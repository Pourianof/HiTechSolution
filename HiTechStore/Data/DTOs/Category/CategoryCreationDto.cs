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