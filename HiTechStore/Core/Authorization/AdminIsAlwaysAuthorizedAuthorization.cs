using HiTechStore.Models;

using Microsoft.AspNetCore.Authorization;

namespace Core.Authorization
{
    public class AdminIsAlwaysAuthorizedAuthorization : IAuthorizationHandler
    {
        public Task HandleAsync(AuthorizationHandlerContext context)
        {
            if (context.User.IsInRole(IdentityRoles.Admin))
            {
                foreach (var requirement in context.Requirements)
                {
                    context.Succeed(requirement);
                }
            }
            return Task.CompletedTask;
        }
    }
}