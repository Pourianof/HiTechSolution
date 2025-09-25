
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using HiTechStore.Core;

namespace HiTechStore.Controllers.ActionFilters
{
    public class HandleModelUpdateActionFilterAttribute<Type, DTO> : ModelAccessorBaseActionFilterAttribute<Type>
        where Type : class, IModel
        where DTO : class
    {
        private readonly System.Type _dtoType;

        public HandleModelUpdateActionFilterAttribute(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _dtoType = typeof(DTO);
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // 1. Check model binding validation
            if (!context.ModelState.IsValid)
            {
                var validationProblem = new ValidationProblemDetails(context.ModelState);
                context.Result = new BadRequestObjectResult(validationProblem);
                return;
            }

            var modelName = EntityName;
            var model = context.ActionArguments.Values.FirstOrDefault(a => a?.GetType() == _dtoType);

            // Check if input model is defined
            if (model == null)
            {
                context.Result = new BadRequestObjectResult($"Invalid {modelName}.");
                return;
            }

            var routeId = context.RouteData.Values["id"] as string;
            // Check if route ID is valid
            if (int.TryParse(routeId, out var routeIdValue))
            {
                var entity = UnitOfWork.Products.GetByIdAsync(routeIdValue).Result;

                // Check if there is corresponding entity for that route ID
                if (entity == null)
                {
                    context.ModelState.AddModelError("", $"{modelName} not found.");
                    var error = new ValidationProblemDetails(context.ModelState);
                    context.Result = new NotFoundObjectResult(error);
                    return;
                }
                // // Check if input model ID matches entity ID
                // else if (model.GetId() != 0 && entity.GetId() != model.GetId())
                // {
                //     context.ModelState.AddModelError("", $"{modelName} ID mismatch.");
                //     var error = new ValidationProblemDetails(context.ModelState);
                //     context.Result = new ConflictObjectResult(error);
                //     return;
                // }

                context.HttpContext.Items["model"] = entity;
            }
        }
    }
}