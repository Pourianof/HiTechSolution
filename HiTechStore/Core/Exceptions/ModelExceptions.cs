namespace HiTechStore.Core.Exceptions;

public class PropertyValueTypeDismatchException : CheckedException
{
    public PropertyValueTypeDismatchException(string message) : base(message, StatusCodes.Status500InternalServerError) { }
    public PropertyValueTypeDismatchException(string message, Exception innerExecption) : base(message, StatusCodes.Status400BadRequest, innerExecption) { }
}