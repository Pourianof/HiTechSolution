using HiTechStore.Core.Auth;
using HiTechStore.Core.Exceptions;
using HiTechStore.Core.Services.Authorization;
using HiTechStore.Models;

namespace HiTechStore.Core.Services;

public class ServiceBase(
    IAuthorizationService authorizationService,
    ICurrentUserProvider currentUserProvider
)
{
    protected string? UserId => currentUserProvider.UserId;

    protected dynamic Unauthorized(
        string? detail = default
    )
    {
        throw new NotAllowedException(
            detail: detail ?? "You are not authorized for this action"
        );

    }

    protected async Task<User> GetUser()
    {
        if (currentUserProvider.UserId is null)
        {
            return Unauthorized();
        }

        var user = await authorizationService.GetUserByIdAsync(currentUserProvider.UserId);

        if (user is null)
        {
            return Unauthorized();
        }

        return user;
    }

    protected async Task<User?> GetUserOrDefault()
    {
        if (currentUserProvider.UserId is null)
        {
            return default;
        }

        return await authorizationService.GetUserByIdAsync(currentUserProvider.UserId);
    }
}