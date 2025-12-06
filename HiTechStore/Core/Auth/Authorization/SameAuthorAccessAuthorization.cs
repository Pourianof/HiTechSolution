using System.Security.Claims;

using HiTechStore.Core.Auth.Authorization.Requirements;

using Microsoft.AspNetCore.Authorization;

namespace HiTechStore.Core.Auth.Authorization
{
    public class SameAuthorAccessAuthorization : AuthorizationHandler<SameAuthorAccessRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, SameAuthorAccessRequirement requirement)
        {
            var user = context.User;
            var resourceUserId = context.Resource?.ToString();

            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!string.IsNullOrEmpty(resourceUserId) && !string.IsNullOrEmpty(userId))
            {
                if (context.User.FindFirstValue(ClaimTypes.NameIdentifier) == resourceUserId)
                {
                    context.Succeed(requirement);
                }
                return Task.CompletedTask;
            }
            return Task.CompletedTask;
        }
    }
}