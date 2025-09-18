using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

using HiTechStore.Data.DTOs.Binders;

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
        [Required]
        [FromJson]
        public IEnumerable<CategoryPropertyEntriesDto>? Properties { get; set; }
    }

    public class CategoryPropertyEntriesDto
    {
        [Required]
        [MinLength(2)]
        [JsonPropertyName("name")]
        public string? Name { get; set; }
        [Required]
        [MinLength(10)]
        [JsonPropertyName("description")]
        public string? Description { get; set; }
    }
}