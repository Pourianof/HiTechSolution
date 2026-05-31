using System.ComponentModel.DataAnnotations;

namespace HiTechStore.Infrastructure.Data.DTOs.Brand;

public class BrandCreationDto
{
    [Required]
    [MinLength(2)]
    public string? name { get; set; }
    public IFormFile? Image { get; set; }

}