using System.Security.Claims;

using HiTechStore.ApiTokenHandler.Dtos;

namespace HiTechStore.ApiTokenHandler.Core;

public interface ITokenHandler
{
    Task<IssuedTokensDto> IssueToken(IEnumerable<Claim> claims, string userId, DateTime? jwtTokenExpiration, DateTime? refreshTokenExpiration = default);
    Task<string> IssueTokenForRefreshToken(string refreshToken, IEnumerable<Claim> claims, DateTime? expiration);
    Task<bool> IsJwtTokenAuthorized(IEnumerable<Claim> claims);
    Task RevokeRefreshToken(IEnumerable<Claim> claims);
    Task<string?> GetRefreshTokenUserId(string token);
}
