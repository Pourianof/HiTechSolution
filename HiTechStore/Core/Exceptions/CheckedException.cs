using HiTechStore.Helpers.Types;

namespace HiTechStore.Core.Exceptions;

public class CheckedException : Exception
{
    public bool IsChecked { get; } = true;
    public int StatusCode { get; init; }
    public CheckedException(string message) : base(message) { }
    public CheckedException(string message, int statusCode) : base(message)
    {
        StatusCode = statusCode;
    }
    public CheckedException(string message, int statusCode, Exception innerException) : base(message, innerException)
    {
        StatusCode = statusCode;
    }
}

public static class CheckedExceptionExtension
{
    public static bool IsChecked(this Exception exception)
    {
        if (exception is CheckedException)
        {
            return true;
        }

        var checkedEx = exception.GetBaseExceptionOfType<CheckedException>();

        return checkedEx is not null;

    }
}