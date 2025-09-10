
namespace HiTechStore.Core.Exceptions;

public class SavingFileException : CheckedException
{
    public SavingFileException(string message) : base(message, StatusCodes.Status500InternalServerError) { }
    public SavingFileException(string message, Exception innerExecption) : base(message, StatusCodes.Status500InternalServerError, innerExecption) { }
}