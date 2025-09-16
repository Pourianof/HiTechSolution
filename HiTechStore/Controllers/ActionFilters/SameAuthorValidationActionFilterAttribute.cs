using Core.Authorization.Requirements;

using HiTechStore.Core;
using HiTechStore.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HiTechStore.Controllers.ActionFilters
{
    public class SameAuthorValidationActionFilterAttribute<Type> : ModelAccessorBaseActionFilterAttribute<Type>
    where Type : class, IModel
    {
        private readonly IAuthorizationService _authorizationService;

        public SameAuthorValidationActionFilterAttribute(IAuthorizationService authorizationService, IUnitOfWork unitOfWork)
        : base(unitOfWork)
        {
            _authorizationService = authorizationService;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var resourceId = context.RouteData.Values["id"]?.ToString();
            var user = context.HttpContext.User;
            if (int.TryParse(resourceId, out var resourceIdValue))
            {
                var resource = DbSet.GetByIdAsync(resourceIdValue).Result as Product;
                if (resource is null)
                {
                    context.Result = new NotFoundResult();
                    return;
                }
                var isAuthorized = _authorizationService.AuthorizeAsync(user, resource?.AuthorId, new SameAuthorAccessRequirement()).Result;

                if (!isAuthorized.Succeeded)
                {
                    context.Result = new ForbidResult();
                }
            }
            else
            {
                context.Result = new BadRequestResult();
            }
        }
    }
}