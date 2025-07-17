
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using HiTechStore.Core;
using HiTechStore.Core.Repositories;
using HiTechStore.Controllers.ActionFilters.Exceptions;
using HiTechStore.Helpers.Types;

namespace HiTechStore.Controllers.ActionFilters
{
    public class HandleModelUpdateActionFilterAttribute<Type, DTO> : ActionFilterAttribute
        where Type : class, IModel
        where DTO : class
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly System.Type _entityType;
        private readonly IRepository<Type> _dbSet;
        private readonly System.Type _dtoType;

        public HandleModelUpdateActionFilterAttribute(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _entityType = typeof(Type);
            _dtoType = typeof(DTO);
            var dbSet = _unitOfWork.GetRepositoryOfType<Type>();

            if (dbSet is null)
            {
                throw new NotExistedDbSetOfProvidedEntityTypeException(_entityType);
            }

            _dbSet = dbSet;
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

            var modelName = _entityType.Name;
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
                var entity = _dbSet.GetByIdAsync(routeIdValue).Result;

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