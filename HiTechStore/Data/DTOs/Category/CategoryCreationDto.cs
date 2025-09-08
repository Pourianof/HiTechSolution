using System.ComponentModel.DataAnnotations;

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
        public int? ParentCategoryId { get; set; }
    }
}