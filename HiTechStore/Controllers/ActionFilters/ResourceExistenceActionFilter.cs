using HiTechStore.Core;
using HiTechStore.Core.Repositories;
using HiTechStore.Data.Repositories;
using HiTechStore.Helpers.Types;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HiTechStore.Controllers.ActionFilters;

public class ResourceExistenceActionFilterAttribute<TModel> : ModelAccessorBaseActionFilterAttribute<TModel>
    where TModel : class, IModel
{

    public ResourceExistenceActionFilterAttribute(IUnitOfWork unitOfWork) : base(unitOfWork)
    { }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        base.OnActionExecuting(context);

        var id = context.RouteData.Values.Where(entry => entry.Key.EndsWith("id") || entry.Key.EndsWith("Id")).FirstOrDefault().Value;

        int? resourceId = null;
        if (id is int)
        {
            resourceId = (int)id;
        }
        else if (id is string && int.TryParse(id as string, out var parsedId))
        {
            resourceId = parsedId;
        }

        if (resourceId is not null)
        {
            var model = Repo.GetModelByIdAsync(resourceId.Value).Result;

            if (model is null)
            {
                var problemDetails = new ProblemDetails { Status = StatusCodes.Status404NotFound, Title = "Resource not found", Detail = $"No {EntityName} found with id {resourceId}" };
                context.Result = new NotFoundObjectResult(problemDetails);
                return;
            }

            context.HttpContext.Items["resource"] = model;
        }
    }
}