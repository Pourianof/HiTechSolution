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
    public async Task<IssuedTokensDto> IssueToken(
        IEnumerable<Claim> claims,
        string userId,
        DateTime? jwtExpiration,
        DateTime? refreshTokenExpiration = default
    )
    {
        var refreshToken = await tokenGenerator.Genreate();

        refreshTokenExpiration ??= DateTime.UtcNow.AddDays(30); // by default 30 day

        await tokenRepository.RegisterToken(
            new()
            {
                Token = refreshToken,
                UserId = userId,
                ExpirateAt = refreshTokenExpiration.Value
            }
        );

        claims = claims.Append(
            new Claim(
                ClaimTypes.Hash, refreshToken
            )
        );

        return new()
        {
            Token = jwtTokenGenerator.CreateJwtToken(claims, jwtExpiration),
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

    public async Task<bool> RevokeRefreshToken(IEnumerable<Claim> claims)
    {
        var hash = claims.FirstOrDefault(c => c.Type == ClaimTypes.Hash)?.Value;

        if (hash is null)
        {
            return false;
        }

        return await tokenRepository.RemoveToken(hash);
    }
    public Task<bool> RevokeRefreshToken(string token)
    {
        return tokenRepository.RemoveByRawToken(token);
    }

    public async Task<string?> GetRefreshTokenUserId(string token)
    {
        var refToken = await tokenRepository.GetTokenFromRaw(token);

        if (refToken is null)
        {
            return null;
        }

        var now = DateTime.UtcNow;
        if (refToken.ExpirateAt < now)
        {
            return null;
        }

        return refToken.UserId;
    }
}
