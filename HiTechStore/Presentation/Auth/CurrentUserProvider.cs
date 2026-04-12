using System.Security.Claims;

using HiTechStore.Core.Auth;


namespace HiTechStore.Presentation.Auth;

public class CurrentUserProvider : ICurrentUserProvider
{
    public string? UserId { get; init; }

    public CurrentUserProvider(ClaimsPrincipal? claimsPrincipal)
    {
        if (claimsPrincipal is not null)
        {
            UserId = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
        }
    }

    public bool IsAuthorized => UserId is not null;
}