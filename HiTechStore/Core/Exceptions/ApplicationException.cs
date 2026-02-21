namespace HiTechStore.Core.Exceptions;

public class ApplicationException : Exception
{
    public string Title { get; private set; }
    public ApplicationException(string title, string detail) : base(detail)
    {
        Title = title;
    }
}


