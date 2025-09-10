using HiTechStore.Core.Exceptions;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HiTechStore.Controllers.ExceptionFilters;

public class GlobalExceptionFilter : ExceptionFilterAttribute
{
    private ILogger<GlobalExceptionFilter> _logger;
    public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger)
    {
        _logger = logger;
    }
    public override void OnException(ExceptionContext context)
    {
        _logger.LogError(context.Exception, "Unhandled exception");

        var exception = context.Exception;

        ProblemDetails problem;
        if (exception.IsChecked())
        {
            problem = new ProblemDetails { Title = "Error", Detail = exception.Message, Status = (exception as CheckedException)?.StatusCode };
        }
        else
        {

            problem = new ProblemDetails { Title = "Error", Detail = "Some error happened.", Status = StatusCodes.Status500InternalServerError };
        }
        context.Result = new ObjectResult(problem) { StatusCode = problem.Status };
    }
}