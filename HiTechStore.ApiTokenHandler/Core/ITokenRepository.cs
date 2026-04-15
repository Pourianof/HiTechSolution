using HiTechStore.ApiTokenHandler.Core.Models;

namespace HiTechStore.ApiTokenHandler.Core;

public interface ITokenRepository
{
    Task<string> RegisterToken(string token, string userId);
    Task<RefreshToken?> GetTokenFromHash(string token);
    Task<RefreshToken?> GetTokenFromRaw(string token);
    Task<IEnumerable<RefreshToken>> GetTokensForUser(string userId);
    Task<bool> RemoveToken(string token);

}