using System.Net;

using Microsoft.AspNetCore.Mvc;

namespace HiTechStore.Core.Exceptions;

public class ApplicationException : Exception
{
    public string Title { get; private set; }
    public HttpStatusCode Status { get; private set; }
    public string Detail => Message;
    public ApplicationException(string title, string detail, HttpStatusCode httpStatusCode) : base(detail)
    {
        Title = title;
        Status = httpStatusCode;
    }

    public ApplicationException(string title, string detail) : base(detail)
    {
        Title = title;
        Status = HttpStatusCode.BadRequest;
    }

    virtual public ProblemDetails ProvideProblemDetails()
    {
        return new ProblemDetails
        {
            Title = Title,
            Detail = Message,
            Status = (int)Status
        };
    }
}


