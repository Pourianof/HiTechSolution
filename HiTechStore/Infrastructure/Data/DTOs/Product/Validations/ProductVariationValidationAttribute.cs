using System.ComponentModel.DataAnnotations;

using HiTechStore.Presentation.Requests;

namespace HiTechStore.Infrastructure.Data.DTOs.Product.Validations;

public class ProductVariationValidationAttribute : ValidationAttribute
{

    protected override ValidationResult IsValid(object? value, ValidationContext context)
    {
        if (value is IEnumerable<ProductVariationCreationRequest> variations)
        {

            if (variations.DistinctBy(v => v.Color).Count() != variations.Count())
            {
                return new ValidationResult(
                    $"Cannot specify multiple variation with same {nameof(ProductVariationCreationRequest.Color)}",
                    [nameof(ProductVariationCreationRequest.Color)]
                );
            }

            return ValidationResult.Success!;

        }
        return new ValidationResult($"{nameof(ProductCreationRequest.Variations)} must be array of product-variation");
    }
}