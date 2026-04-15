namespace HiTechStore.ApiTokenHandler.Core;

public interface IRandomSecureTokenGenerator
{
    Task<string> Genreate();
}