
using System.Security.Cryptography;
using System.Text;

using HiTechStore.ApiTokenHandler.Core;
using HiTechStore.ApiTokenHandler.Core.Models;

using Microsoft.EntityFrameworkCore;

namespace HiTechStore.ApiTokenHandler.Infrastructure;

internal class EfTokenRepository(AuthTokensDbContext dbContext) : ITokenRepository
{
    // i putted hash strategy in repository to define a
    // clear relation between the hashing token and stored tokens
    // if hash startegy changed, we revoke old tokens,
    // but in other side, if hash strategy changes, so old tokens
    // will be inaccessible automatically and we need no further action
    // but i saw this approach better
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
        var hashed = await Hash(token);

        return await dbContext.RefreshTokens.FirstOrDefaultAsync(
            rt => rt.Token == hashed
        );
    }

    public Task<RefreshToken?> GetTokenFromRaw(string token)
    {
        return dbContext.RefreshTokens.FirstOrDefaultAsync(
           rt => rt.Token == token
        );
    }

    public async Task<IEnumerable<RefreshToken>> GetTokensForUser(string userId)
    {
        return await dbContext.RefreshTokens.Where(
           rt => rt.UserId == userId
        ).ToListAsync();
    }

    public Task<string> HashTokenAsync(string token)
    {
        return Hash(token);
    }

    public async Task<string> RegisterToken(string token, string userId)
    {
        var hashed = await Hash(token);
        await dbContext.RefreshTokens.AddAsync(
           new()
           {
               Token = hashed,
               UserId = userId
           }
        );

        return hashed;
    }

    public async Task<bool> RemoveToken(string token)
    {
        return await dbContext.RefreshTokens.Where(e => e.Token == token).ExecuteDeleteAsync() > 0;
    }
}