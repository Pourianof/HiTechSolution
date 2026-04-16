using HiTechStore.ApiTokenHandler.Core.Models;

namespace HiTechStore.ApiTokenHandler.Core;

public interface ITokenRepository
{
    Task<string> RegisterToken(RefreshToken token);
    Task<RefreshToken?> GetTokenFromHash(string token);
    Task<RefreshToken?> GetTokenFromRaw(string token);
    Task<IEnumerable<RefreshToken>> GetTokensForUser(string userId);
    Task<bool> RemoveToken(string token);
    Task<bool> RemoveByRawToken(string token);
}