using System.ComponentModel.DataAnnotations;

namespace HiTechStore.Core.Validators;

[AttributeUsage(AttributeTargets.Property)]
public class ValidExtensionsAttribute : ValidationAttribute
{
    private string[] _validExtensions;

    public ValidExtensionsAttribute(string[] validExtensions)
    {
        _validExtensions = validExtensions.Select((v) => v.ToLower()).ToArray();
    }

    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        var file = value as IFormFile;
        if (file != null)
        {
            var extension = Path.GetExtension(file.FileName).ToLower();
            if (!_validExtensions.Any(v => v == extension || $".{v}" == extension))
            {
                return new ValidationResult($"File format is not valid: {string.Join(", ", _validExtensions)}");
            }
        }

        return ValidationResult.Success!;
    }
}