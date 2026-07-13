using System.Security.Claims;
using System.Text.Encodings.Web;

using HiTechStore.IntegrationTests.Infrastructure;

using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HiTechStore.IntegrationTests;

public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string SchemeName = "TestAuth";
    public TestAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder) { }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        string? token = Context.Request.Headers.Authorization.ToString().Replace("Bearer ", "");

        if (string.IsNullOrEmpty(token))
        {
            token = Context.Request.Query["access_token"];
        }

        if (string.IsNullOrEmpty(token) || !token.StartsWith(TestJwtTokenGenerator.SchemePrefix))
        {
            return Task.FromResult(AuthenticateResult.Fail("No test token provided."));
        }

        var parts = token.Split(TestJwtTokenGenerator.Seperator);
        var userId = parts[1];

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId)
        };

        var identity = new ClaimsIdentity(claims, SchemeName);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, SchemeName);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}