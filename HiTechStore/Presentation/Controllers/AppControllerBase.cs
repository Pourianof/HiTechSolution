using System.ComponentModel.DataAnnotations;

using HiTechStore.Core.Helpers.Result;
using HiTechStore.Presentation.Helpers.Result;

using Microsoft.AspNetCore.Mvc;

namespace HiTechStore.Presentation.Controllers;

[ApiController]
public class AppControllerBase : ControllerBase
{
    protected BadRequestObjectResult ValidationResult(IEnumerable<ValidationResultError> errors, string? title = default)
    {
        var problem = ResultValidationMapper.ToValidationProblemDetails(errors, title!);
        return BadRequest(problem);
    }

    protected ObjectResult ResultCheck<T>(Result<T> result, string? title = default)
    {
        // If there are validation errors, convert them to ModelState and return a ValidationProblemDetails
        if (result.Errors != null && result.Errors.OfType<ValidationResultError>().Any())
        {
            return ValidationResult(result.Errors.OfType<ValidationResultError>(), title);
        }

        if (result.HasError)
        {
            return new BadRequestObjectResult(
                new
                {
                    Title = "Bad request",
                    result.Errors
                }
            );
        }

        return Ok(result.Value);
    }

    protected ObjectResult ResultCheck(Result result, object response, string? title = default)
    {
        // If there are validation errors, convert them to ModelState and return a ValidationProblemDetails
        if (result.Errors != null && result.Errors.OfType<ValidationResultError>().Any())
        {
            return ValidationResult(result.Errors.OfType<ValidationResultError>(), title);
        }

        if (result.HasError)
        {
            return new BadRequestObjectResult(
                new
                {
                    Title = "Bad request",
                    result.Errors
                }
            );
        }

        return Ok(response);
    }
}