using System;

namespace HiTechStore.ApiTokenHandler.Core;

public interface IAuthTokensRepoistory
{
    Task RegisterRefreshTokenForUser(string userId, string token);
    Task RevokeRefreshTokenForUser(string userId, string token);
}
