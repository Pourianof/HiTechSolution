using System.ComponentModel.DataAnnotations;
using System.Numerics;
using System.Globalization;

namespace HiTechStore.Infrastructure.Data.DTOs.Validations;

class NumberValidatorHelper
{
    // Helper: detect if the runtime type implements System.Numerics.INumber<>
    private bool ImplementsINumber(Type t)
    {
        var inumberOpen = typeof(INumber<>);
        return t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == inumberOpen);
    }

    private object _value;
    private decimal _numericValue;
    public bool IsValidNumber { get; private set; } = false;
    public NumberValidatorHelper(object value)
    {
        _value = value;
        Validate();
    }

    private void Validate()
    {
        var valueType = _value.GetType();

        // If it's a string, try parsing
        if (_value is string s)
        {
            if (!decimal.TryParse(s, NumberStyles.Number, CultureInfo.InvariantCulture, out _numericValue))
                return;
        }

        else if (ImplementsINumber(valueType) || _value is IConvertible)
        {
            try
            {
                _numericValue = Convert.ToDecimal(_value, CultureInfo.InvariantCulture);
            }
            catch
            {
                return;
            }
        }
        else
        {
            return;
        }

        IsValidNumber = true;
    }

    public decimal Number => IsValidNumber ? _numericValue : throw new InvalidOperationException("cannot access to invalid object as number");
}

public class NonZeroPositiveNumberAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object? value, ValidationContext context)
    {
        // Null is considered valid here; use [Required] when needed
        if (value is null)
            return ValidationResult.Success!;

        var positiveValidationResult = new ValidationResult($"The {context.DisplayName} field must be a Non-Zero positive number");

        var validation = new NumberValidatorHelper(value);

        if (!validation.IsValidNumber)
        {
            return positiveValidationResult;
        }

        return validation.Number > 0m
            ? ValidationResult.Success!
            : positiveValidationResult;
    }
}


public class PositiveNumberAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object? value, ValidationContext context)
    {
        // Null is considered valid here; use [Required] when needed
        if (value is null)
            return ValidationResult.Success!;

        var positiveValidationResult = new ValidationResult($"The {context.DisplayName} field must be a positive number");

        var validation = new NumberValidatorHelper(value);

        if (!validation.IsValidNumber)
        {
            return positiveValidationResult;
        }

        return validation.Number >= 0m
            ? ValidationResult.Success!
            : positiveValidationResult;
    }
}