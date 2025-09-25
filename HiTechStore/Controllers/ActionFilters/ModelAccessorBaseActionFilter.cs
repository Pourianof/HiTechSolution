using HiTechStore.Core;

using Microsoft.AspNetCore.Mvc.Filters;

namespace HiTechStore.Controllers.ActionFilters
{
    public abstract class ModelAccessorBaseActionFilterAttribute<Type> : ActionFilterAttribute
    where Type : class, IModel
    {

        protected readonly IUnitOfWork UnitOfWork;
        private readonly System.Type _entityType;

        public ModelAccessorBaseActionFilterAttribute(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
            _entityType = typeof(Type);
        }

        protected string EntityName => _entityType.Name;

        protected async Task CompleteDbWork()
        {
            await UnitOfWork.Complete();
        }

    }
}