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

            var problem = httpContext.Request.Method.ToLower() switch
            {
                "post" => new ProblemDetails
                {
                    Title = "Bad input data",
                    Detail = $"Duplicated '{constraintColumn ?? "<unknown-property>"}' value in {tableName}",
                    Status = StatusCodes.Status409Conflict
                },
                "delete" => new ProblemDetails
                {
                    Title = "Data conflict",
                    Detail = $"There is some dependency in database to the item you wanna delete which not allowable",
                    Status = StatusCodes.Status409Conflict
                },
                _ => new ProblemDetails
                {
                    Title = "Data conflict",
                    Detail = $"Action you wanna do could not happen in database",
                    Status = StatusCodes.Status409Conflict
                }
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

        var lastPart = constraint.Split('_').ElementAt(^1);

        if (lastPart is string)
        {
            var normalizedStr = "normalized";
            if (lastPart.ToLower().StartsWith(normalizedStr))
            {
                return lastPart.Substring(normalizedStr.Length);
            }

            return lastPart;

        }

        return null;

    }
}