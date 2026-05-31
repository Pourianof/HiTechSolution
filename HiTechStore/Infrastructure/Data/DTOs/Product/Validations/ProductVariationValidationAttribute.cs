using System.ComponentModel.DataAnnotations;

using HiTechStore.DTOs.Product;

namespace HiTechStore.Infrastructure.Data.DTOs.Product.Validations;

public class ProductVariationValidationAttribute : ValidationAttribute
{

    protected override ValidationResult IsValid(object? value, ValidationContext context)
    {
        if (value is IEnumerable<ProductVariationCreationDto> variations)
        {

            if (variations.DistinctBy(v => v.Color).Count() != variations.Count())
            {
                return new ValidationResult(
                    $"Cannot specify multiple variation with same {nameof(ProductVariationCreationDto.Color)}",
                    [nameof(ProductVariationCreationDto.Color)]
                );
            }

            return ValidationResult.Success!;

        }
        return new ValidationResult("media must be array of product-variation");
    }
}