using HiTechStore.Core.Helpers.Result;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace HiTechStore.Presentation.Helpers.Result;

public static class ResultValidationMapper
{
    public static ValidationProblemDetails ToValidationProblemDetails<T>(Result<T> result, string title = "Validation failed")
    {
        var errors = result?.Errors ?? Enumerable.Empty<ResultError>();
        return ToValidationProblemDetails(errors, title);
    }

    public static ValidationProblemDetails ToValidationProblemDetails(IEnumerable<ResultError> errors, string title = "Validation failed")
    {
        var ms = new ModelStateDictionary();

        foreach (var e in errors.OfType<ValidationResultError>())
        {
            var key = e.FieldName ?? e.Code ?? "Validation";
            var message = e.Description ?? e.Title ?? "Invalid value";
            ms.AddModelError(key, message);
        }

        return new ValidationProblemDetails(ms)
        {
            Title = title,
            Status = StatusCodes.Status400BadRequest,
            Detail = "One or more validation errors occurred."
        };
    }
}
