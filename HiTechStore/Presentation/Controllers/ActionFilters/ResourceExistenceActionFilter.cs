using HiTechStore.Core;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HiTechStore.Presentation.Controllers.ActionFilters;

public class ResourceExistenceActionFilterAttribute<TModel, TId> : ModelAccessorBaseActionFilterAttribute<TModel, TId>
    where TModel : class, IModel
    where TId : struct
{

    public ResourceExistenceActionFilterAttribute(IUnitOfWork unitOfWork) : base(unitOfWork)
    { }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        base.OnActionExecuting(context);

        var id = context.RouteData.Values.FirstOrDefault(entry => entry.Key.EndsWith("id") || entry.Key.EndsWith("Id")).Value;

        TId? resourceId = null;
        if (id is TId)
        {
            resourceId = (TId)id;
        }
        else if (id is string)
        {
            try
            {
                var parsed = Convert.ChangeType(id, typeof(TId));
                resourceId = (TId)parsed;
            }
            catch
            {
                context.Result = new BadRequestObjectResult(new ProblemDetails
                {
                    Title = "Invalid id",
                    Detail = $"Specified id is not type of {typeof(TId).Name}"
                });

                return;
            }
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

public class ResourceExistenceActionFilterAttribute<TModel> :
    ResourceExistenceActionFilterAttribute<TModel, int>
    where TModel : class, IModel
{
    public ResourceExistenceActionFilterAttribute(IUnitOfWork unitOfWork) : base(unitOfWork)
    {
    }
}