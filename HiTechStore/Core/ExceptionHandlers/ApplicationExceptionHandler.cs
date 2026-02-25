using System;

using HiTechStore.Core.Exceptions;

using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace HiTechStore.Core.ExceptionHandlers;

public class ApplicationExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        ProblemDetails problemDetails;

        switch (exception)
        {
            case ModelException ex:
                {
                    var validationProblemDetails = new ValidationProblemDetails(
                        new Dictionary<string, string[]>
                        {
                        { ex.FieldName, new[] { ex.Message } }
                        })
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Title = "Validation Error"
                    };

                    httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await httpContext.Response.WriteAsJsonAsync(validationProblemDetails, cancellationToken);
                    return true;
                }

            case NotAllowedException ex:
                {
                    problemDetails = new ProblemDetails
                    {
                        Title = ex.Title,
                        Detail = ex.Message,
                        Status = StatusCodes.Status401Unauthorized
                    };

                    httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await httpContext.Response.WriteAsJsonAsync(new UnauthorizedObjectResult(problemDetails), cancellationToken);
                    return true;
                }

            case Core.Exceptions.ApplicationException ex:
                {
                    problemDetails = new ProblemDetails
                    {
                        Title = ex.Title,
                        Detail = ex.Message,
                        Status = StatusCodes.Status400BadRequest
                    };

                    httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
                    return true;
                }

            default:
                {
                    problemDetails = new ProblemDetails
                    {
                        Title = "Server Error",
                        Detail = "An unexpected error occurred.",
                        Status = StatusCodes.Status500InternalServerError
                    };

                    httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
                    return true;
                }
        }
    }
}

