
using System.Security.Cryptography;
using System.Text;

using HiTechStore.ApiTokenHandler.Core;
using HiTechStore.ApiTokenHandler.Core.Models;

namespace HiTechStore.ApiTokenHandler.UnitTests.Helpers;

public class InMemoryTokenRepository : ITokenRepository
{
    private List<RefreshToken> _tokenStore = new();

    public static Task<string> Hash(string input)
    {
        using (var sha256 = SHA256.Create())
        {
            var bytes = Encoding.UTF8.GetBytes(input);
            var hash = sha256.ComputeHash(bytes);

            return Task.FromResult(Convert.ToHexString(hash));
        }
    }

    public async Task<RefreshToken?> GetTokenFromHash(string token)
    {
        return _tokenStore.FirstOrDefault(
           rt => rt.Token == token
        );
    }

    public async Task<RefreshToken?> GetTokenFromRaw(string token)
    {
        var hashed = await Hash(token);

        return _tokenStore.FirstOrDefault(
            rt => rt.Token == hashed
        );
    }

    public async Task<IEnumerable<RefreshToken>> GetTokensForUser(string userId)
    {
        return _tokenStore.Where(
           rt => rt.UserId == userId
        ).ToList();
    }

    public async Task<string> RegisterToken(RefreshToken refToken)
    {
        refToken.Token = await Hash(refToken.Token!);
        _tokenStore.Add(
           refToken
        );

        return refToken.Token;
    }

    public async Task<bool> RemoveByRawToken(string token)
    {
        var hashed = await Hash(token);
        return _tokenStore.RemoveAll(
            rt => rt.Token == hashed
        ) > 0;
    }

    public async Task<bool> RemoveToken(string token)
    {
        return _tokenStore.RemoveAll(
            rt => rt.Token == token
        ) > 0;
    }
}