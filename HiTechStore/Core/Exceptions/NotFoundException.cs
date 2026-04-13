using System;

namespace HiTechStore.Core.Exceptions;

public class NotFoundException : ApplicationException
{
    public NotFoundException(string title, string detail) : base(title, detail, System.Net.HttpStatusCode.NotFound)
    {
    }
    public NotFoundException(string detail) : base("Not found", detail, System.Net.HttpStatusCode.NotFound)
    {
    }
}
