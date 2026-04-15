namespace HiTechStore.ApiTokenHandler.Core.Exceptions;



public class TokenHandlerException : Exception
{
    public class ExpiredTokenException : TokenHandlerException { }

    public class NotFoundRefreshToken : TokenHandlerException { }
}