
using System.Security.Claims;

using HiTechStore.ApiTokenHandler.Core;
using HiTechStore.ApiTokenHandler.Core.Exceptions;
using HiTechStore.ApiTokenHandler.Infrastructure;
using HiTechStore.ApiTokenHandler.UnitTests.Helpers;

namespace HiTechStore.ApiTokenHandler.UnitTests.Core;

public class JwtTokenHandlerTests
{
    public InMemoryTokenRepository TokenRepository = new();
    public IJwtTokenGenerator TokenGenerator = new JwtTokenGenerator(
        "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX",
        "me",
        "you"
    );

    private ITokenHandler _tokenHandlerUnderTest;
    public JwtTokenHandlerTests()
    {
        _tokenHandlerUnderTest = new JwtTokenHandler(TokenRepository, new RandomTokenGenerator(), TokenGenerator);
    }

    [Fact]
    public async Task IsJwtTokenAuthorized_WithExpiredToken_ReturnFalse()
    {

        // Arrange
        var token = "111111";
        var userId = "xxxxx";
        var expiredDate = DateTime.UtcNow.AddDays(-1);

        var hashedToken = await TokenRepository.RegisterToken(
            new()
            {
                Token = token,
                UserId = userId,
                ExpirateAt = expiredDate
            }
        );

        IEnumerable<Claim> claims = [
            new Claim(
                ClaimTypes.NameIdentifier, userId
            ),
            new Claim(
                ClaimTypes.Hash, hashedToken
            )
        ];

        // Action
        var result = await _tokenHandlerUnderTest.IsJwtTokenAuthorized(claims);


        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task IsJwtTokenAuthorized_WithActiveToken_ReturnTrue()
    {

        // Arrange
        var token = "111111";
        var userId = "xxxxx";
        var expiredDate = DateTime.UtcNow.AddDays(1);

        var hashedToken = await TokenRepository.RegisterToken(
            new()
            {
                Token = token,
                UserId = userId,
                ExpirateAt = expiredDate
            }
        );

        IEnumerable<Claim> claims = [
            new Claim(
                ClaimTypes.NameIdentifier, userId
            ),
            new Claim(
                ClaimTypes.Hash, hashedToken
            )
        ];

        // Action
        var result = await _tokenHandlerUnderTest.IsJwtTokenAuthorized(claims);


        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task GetRefreshTokenUserId_WithValidRefreshToken_ReturnUserId()
    {
        // Arrange
        var token = "111111";
        var userId = "xxxxx";
        var expiredDate = DateTime.UtcNow.AddDays(1);

        await TokenRepository.RegisterToken(
            new()
            {
                Token = token,
                UserId = userId,
                ExpirateAt = expiredDate
            }
        );

        // Action
        var tokenOwnerId = await _tokenHandlerUnderTest.GetRefreshTokenUserId(token);

        // Assert
        Assert.Equal(userId, tokenOwnerId);
    }

    [Fact]
    public async Task GetRefreshTokenUserId_WithExpiredRefreshToken_ReturnNullUserId()
    {
        // Arrange
        var token = "111111";
        var userId = "xxxxx";
        var expiredDate = DateTime.UtcNow.AddDays(-1);

        await TokenRepository.RegisterToken(
            new()
            {
                Token = token,
                UserId = userId,
                ExpirateAt = expiredDate
            }
        );

        // Action
        var tokenOwnerId = await _tokenHandlerUnderTest.GetRefreshTokenUserId(token);

        // Assert
        Assert.Null(tokenOwnerId);
    }

    [Fact]
    public async Task IssueToken_WithCustomData_ReturnValidTokens()
    {
        // Arrange
        var userId = "xxxxx";
        var expiredDate = DateTime.UtcNow.AddDays(1);


        // Action
        var tokens = await _tokenHandlerUnderTest.IssueToken(
            [],
            userId,
            expiredDate
        );
        var tokenOwner = await _tokenHandlerUnderTest.GetRefreshTokenUserId(tokens.RefreshToken!);

        // Assert
        Assert.Equal(tokenOwner, userId);
        Assert.NotNull(tokens.RefreshToken);
        Assert.NotEmpty(tokens.RefreshToken);
    }

    [Fact]
    public async Task IssueTokenForRefreshToken_WithCustomData_ThrowsNotFoundRefreshTokenException()
    {
        // Arrange
        var token = "111111";
        var expiredDate = DateTime.UtcNow.AddDays(1);
        // no ref token registered

        // Action
        var issueAction = async () => await _tokenHandlerUnderTest.IssueTokenForRefreshToken(
                                token,
                                [],
                                expiredDate
                            );

        // Assert
        await Assert.ThrowsAsync<TokenHandlerException.NotFoundRefreshToken>(issueAction);

    }

    [Fact]
    public async Task IssueTokenForRefreshToken_WhenRefreshTokenHasExpired_ThrowsExpiredTokenException()
    {
        // Arrange
        var token = "111111";
        var userId = "xxxxx";
        var expiredDate = DateTime.UtcNow.AddDays(-1);

        await TokenRepository.RegisterToken(
            new()
            {
                Token = token,
                UserId = userId,
                ExpirateAt = expiredDate
            }
        );

        // Action
        var issueAction = async () => await _tokenHandlerUnderTest.IssueTokenForRefreshToken(
                                token,
                                [],
                                expiredDate
                            );

        // Assert
        await Assert.ThrowsAsync<TokenHandlerException.ExpiredTokenException>(issueAction);

    }

    [Fact]
    public async Task IssueTokenForRefreshToken_WithCustomData_ReturnValidTokens()
    {
        // Arrange
        var token = "111111";
        var userId = "xxxxx";
        var expiredDate = DateTime.UtcNow.AddDays(1);

        await TokenRepository.RegisterToken(
                    new()
                    {
                        Token = token,
                        UserId = userId,
                        ExpirateAt = expiredDate
                    }
                );

        // Action
        var jwtToken = await _tokenHandlerUnderTest.IssueTokenForRefreshToken(
            token,
            [],
            expiredDate
        );
        var rToken = await TokenRepository.GetTokenFromRaw(token);

        // Assert
        Assert.NotNull(jwtToken);
        Assert.NotEmpty(jwtToken);

        Assert.NotNull(rToken);

    }
}