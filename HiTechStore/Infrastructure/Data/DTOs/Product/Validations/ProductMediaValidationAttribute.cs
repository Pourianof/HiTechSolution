using System.ComponentModel.DataAnnotations;

using HiTechStore.Core.Models;

namespace HiTechStore.Infrastructure.Data.DTOs.Product.Validations;

public class ProductMediaValidationAttribute : ValidationAttribute
{

    protected override ValidationResult IsValid(object? value, ValidationContext context)
    {
        if (value is IEnumerable<IFormFile> media)
        {
            if (media.Count() == 0)
            {
                return new ValidationResult("At least one cover image must defined");
            }



            var invalidMedia = media.Where((m) => !MediaTypeHelper.IsValid(m.FileName));
            if (invalidMedia.Count() > 0)
            {
                foreach (var invalid in invalidMedia)
                {
                    return new ValidationResult($"The media '{invalid.FileName}' has not valid type.\nValid types: [{string.Join(", ", MediaTypeHelper.ValidTypes())}]");

                }
            }

            var produceCoverImageProblemDetails = () =>
                    {
                        return new ValidationResult("At least one cover image must define for product");
                    };

            var hasCoverImage = media.Any((m) => MediaTypeHelper.IsImage(m.FileName));
            if (!hasCoverImage)
            {
                return produceCoverImageProblemDetails();
            }

            return ValidationResult.Success!;

        }
        return new ValidationResult("media must be array of files");
    }
}