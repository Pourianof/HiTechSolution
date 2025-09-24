using System.ComponentModel.DataAnnotations;

namespace HiTechStore.Data.DTOs.Brand;

public class CreateBrandDto
{
    [Required]
    [MinLength(2)]
    public string? name { get; set; }
    public IFormFile? Image { get; set; }

}