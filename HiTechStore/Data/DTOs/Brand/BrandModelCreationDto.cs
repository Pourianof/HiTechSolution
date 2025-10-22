using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HiTechStore.Data.DTOs.Brand;

public class BrandModelCreationDto : IValidatableObject
{
    public BrandCreationDto? Brand { get; set; }
    public int? BrandId { get; set; }
    [Required]
    [MinLength(2)]
    public string? Name { get; set; }
    [MinLength(5)]
    public string? Description { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (BrandId is null && Brand is null)
        {
            yield return new ValidationResult($"You must specify the brand which this model is belong to with existing brands({nameof(BrandId)} or create new one({nameof(Brand)}))");
        }
        yield return ValidationResult.Success!;
    }
}