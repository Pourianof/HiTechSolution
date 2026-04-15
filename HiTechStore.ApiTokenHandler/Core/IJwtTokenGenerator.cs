using System.Security.Claims;

namespace HiTechStore.ApiTokenHandler.Core;

public interface IJwtTokenGenerator
{
    string CreateJwtToken(IEnumerable<Claim> claims, DateTime? expiration);
}