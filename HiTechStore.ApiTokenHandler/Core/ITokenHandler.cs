using System.Security.Claims;

using HiTechStore.ApiTokenHandler.Dtos;

namespace HiTechStore.ApiTokenHandler.Core;

public interface ITokenHandler
{
    Task<IssuedTokensDto> IssueToken(IEnumerable<Claim> claims, string userId, DateTime? expiration);
    Task<string> IssueTokenForRefreshToken(string refreshToken, IEnumerable<Claim> claims, DateTime? expiration);
    Task<bool> IsJwtTokenAuthorized(IEnumerable<Claim> claims);
    Task RevokeRefreshToken(IEnumerable<Claim> claims);
}
