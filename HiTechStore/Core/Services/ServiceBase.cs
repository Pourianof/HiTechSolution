using HiTechStore.Core.Common.Interfaces.Presentation;
using HiTechStore.Core.Exceptions;
using HiTechStore.Core.Services.Authorization;
using HiTechStore.Core.Models;

namespace HiTechStore.Core.Services;

public class ServiceBase(
    IAuthorizationService authorizationService,
    ICurrentUserProvider currentUserProvider
)
{
    protected IAuthorizationService AuthorizationService = authorizationService;
    protected string? UserId => currentUserProvider.UserId;
    protected string UserIdOrThrow => currentUserProvider.UserId ?? Unauthorized();

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

        var user = await AuthorizationService.GetUserByIdAsync(currentUserProvider.UserId);

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

        return await AuthorizationService.GetUserByIdAsync(currentUserProvider.UserId);
    }
}