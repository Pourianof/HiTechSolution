namespace HiTechStore.Core.Exceptions;

public class NotAllowedException : ApplicationException
{
    public NotAllowedException(string title = "You are not allowed to perform this action", string detail = "") : base(title, detail)
    {
    }
}
