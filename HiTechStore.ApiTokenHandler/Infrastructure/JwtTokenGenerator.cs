using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using HiTechStore.ApiTokenHandler.Core;

using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace HiTechStore.ApiTokenHandler.Infrastructure;

internal class JwtTokenGenerator : IJwtTokenGenerator
{
    private string? _audience;
    private string _key;
    private string? _issuer;

    // for testing purpose in a offline net mode
    public JwtTokenGenerator(
        string key,
        string? issuer,
        string? audience
    )
    {
        _key = key;
        _issuer = issuer;
        _audience = audience;
    }

    public JwtTokenGenerator(IConfiguration configuration)
    {
        _audience = configuration["Jwt:Audience"];
        _key = configuration["Jwt:Key"]!;
        _issuer = configuration["Jwt:Issuer"];

    }

    public string CreateJwtToken(
            IEnumerable<Claim> claims,
            DateTime? expiration
        )
    {
        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));
        var creds = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256Signature);

        var jwt = new JwtSecurityToken(
            claims: claims,
            expires: expiration,
            signingCredentials: creds,
            audience: _audience,
            issuer: _issuer
        );

        var jwtToken = new JwtSecurityTokenHandler().WriteToken(jwt);

        return jwtToken;
    }

    public IEnumerable<Claim> GetJwtTokenClaims(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ValidateToken(token, new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = false,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your-secret"))
        }, out _);

        return jwt.Claims;
    }
}