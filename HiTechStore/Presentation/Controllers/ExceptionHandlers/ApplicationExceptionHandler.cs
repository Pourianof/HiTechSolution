using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace HiTechStore.Presentation.Controllers.ExceptionHandlers;

public class ApplicationExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        ProblemDetails problemDetails;

        switch (exception)
        {
            case Core.Exceptions.ApplicationException ex:
                {
                    problemDetails = ex.ProvideProblemDetails();

                    httpContext.Response.StatusCode = (int)ex.Status;
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

