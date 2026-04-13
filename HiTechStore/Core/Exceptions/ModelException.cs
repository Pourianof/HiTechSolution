using Microsoft.AspNetCore.Mvc;

namespace HiTechStore.Core.Exceptions;

public class ModelException : ApplicationException
{
    public string FieldName { get; private set; }

    public ModelException(string title, string description, string fieldName)
        : base(title, description, System.Net.HttpStatusCode.BadRequest)
    {
        FieldName = fieldName;
    }

    public override ProblemDetails ProvideProblemDetails()
    {
        return new ValidationProblemDetails(
            new Dictionary<string, string[]>
            {
                { FieldName, new[] { Message } }
            }
        )
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Validation Error"
        };
    }
}
