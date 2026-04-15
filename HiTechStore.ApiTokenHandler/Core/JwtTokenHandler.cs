using System.Security.Claims;

using HiTechStore.ApiTokenHandler.Core.Exceptions;
using HiTechStore.ApiTokenHandler.Dtos;

namespace HiTechStore.ApiTokenHandler.Core;

public class JwtTokenHandler(
    ITokenRepository tokenRepository,
    IRandomSecureTokenGenerator tokenGenerator,
    IJwtTokenGenerator jwtTokenGenerator
) : ITokenHandler
{
    public async Task<IssuedTokensDto> IssueToken(IEnumerable<Claim> claims, string userId, DateTime? expiration)
    {
        var refreshToken = await tokenGenerator.Genreate();

        await tokenRepository.RegisterToken(refreshToken, userId);

        claims = claims.Append(
            new Claim(
                ClaimTypes.Hash, refreshToken
            )
        );

        return new()
        {
            Token = jwtTokenGenerator.CreateJwtToken(claims, expiration),
            RefreshToken = refreshToken
        };
    }

    public async Task<string> IssueTokenForRefreshToken(
        string refreshToken,
        IEnumerable<Claim> claims,
        DateTime? expiration)
    {
        var now = DateTime.UtcNow;

        // check is refresh token valid
        var refToken = await tokenRepository.GetTokenFromRaw(refreshToken);

        if (refToken is null)
        {
            throw new TokenHandlerException.NotFoundRefreshToken();
        }

        if (refToken.ExpirateAt < now)
        {
            throw new TokenHandlerException.ExpiredTokenException();
        }

        claims = claims.Append(
            new Claim(
                ClaimTypes.Hash, refToken.Token!
            )
        );

        // create new token
        var jwtToken = jwtTokenGenerator.CreateJwtToken(claims, expiration);

        return jwtToken;
    }

    public async Task<bool> IsJwtTokenAuthorized(IEnumerable<Claim> claims)
    {
        var hashClaim = claims.FirstOrDefault(
            c => c.Type == ClaimTypes.Hash
        );

        if (hashClaim is null)
        {
            return false;
        }

        var refToken = await tokenRepository.GetTokenFromHash(hashClaim.Value);

        if (refToken is null || refToken.ExpirateAt < DateTime.UtcNow)
        {
            return false;
        }

        return true;
    }

    public async Task RevokeRefreshToken(IEnumerable<Claim> claims)
    {
        var hash = claims.FirstOrDefault(c => c.Type == ClaimTypes.Hash)?.Value;

        if (hash is null)
        {
            return;
        }

        await tokenRepository.RemoveToken(hash);
    }
}
