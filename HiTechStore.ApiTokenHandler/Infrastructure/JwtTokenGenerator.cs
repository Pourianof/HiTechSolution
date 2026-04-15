using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using HiTechStore.ApiTokenHandler.Core;

using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace HiTechStore.ApiTokenHandler.Infrastructure;

internal class JwtTokenGenerator(IConfiguration configuration) : IJwtTokenGenerator
{
    public string CreateJwtToken(
            IEnumerable<Claim> claims,
            DateTime? expiration
        )
    {
        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));
        var creds = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256Signature);

        var jwt = new JwtSecurityToken(
            claims: claims,
            expires: expiration,
            signingCredentials: creds,
            audience: configuration["Jwt:Audience"],
            issuer: configuration["Jwt:Issuer"]
        );

        var jwtToken = new JwtSecurityTokenHandler().WriteToken(jwt);

        return jwtToken;
    }
}