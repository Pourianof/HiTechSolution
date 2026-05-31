using HiTechStore.Core.Exceptions;

using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace HiTechStore.Presentation.Controllers.ExceptionFilters;

public class GlobalExceptionHandler : IExceptionHandler
{
    private ILogger<GlobalExceptionHandler> _logger;
    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }
    public ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Unhandled exception");


        ProblemDetails problem;
        if (exception.IsChecked())
        {
            problem = new ProblemDetails { Title = "Error", Detail = exception.Message, Status = (exception as CheckedException)?.StatusCode };
        }
        else
        {

            problem = new ProblemDetails { Title = "Error", Detail = "Some error happened.", Status = StatusCodes.Status500InternalServerError };
        }

        context.Response.StatusCode = problem.Status ?? 500;
        context.Response.WriteAsJsonAsync(problem);


        return ValueTask.FromResult(true);
    }
}