using System.Text.RegularExpressions;

using HiTechStore.Helpers.Types;

using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

using Npgsql;


namespace HiTechStore.Core.ExceptionHandlers;

public class PgDbExceptionHandler : IExceptionHandler
{
    public ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {

        var pgException = exception.GetBaseExceptionOfType<PostgresException>();
        if (pgException == null)
        {
            return ValueTask.FromResult(false);
        }

        // Unique Constraint violation
        if (pgException.ErrorCode == -2147467259)
        {
            var tableName = pgException.TableName;
            var constraintName = pgException.ConstraintName;

            var constraintColumn = GetConstraintColumnName(constraintName);

            var problem = new ProblemDetails
            {
                Title = "Bad input data",
                Detail = $"Duplicated '{constraintColumn ?? "<unknown-property>"}' value in {tableName}",
                Status = StatusCodes.Status409Conflict
            };

            httpContext.Response.WriteAsJsonAsync(problem).Wait();

            return ValueTask.FromResult(true);
        }

        return ValueTask.FromResult(false);


    }

    private string? GetConstraintColumnName(string? constraint)
    {
        if (constraint is null)
        {
            return null;
        }

        var pattern = @"^IX_\w+_(Normalized)(\w+)$";
        var match = Regex.Match(constraint, pattern, RegexOptions.IgnoreCase);

        if (match.Success)
        {
            if (match.Groups[1].Value.ToLower() == "normalized")
            {
                return match.Groups[2].Value;
            }

            return match.Groups[1].Value;

        }

        return null;

    }
}