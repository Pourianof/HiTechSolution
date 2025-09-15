using HiTechStore.Controllers.ActionFilters.Exceptions;
using HiTechStore.Core;
using HiTechStore.Core.Repositories;
using HiTechStore.Helpers.Types;

using Microsoft.AspNetCore.Mvc.Filters;

namespace HiTechStore.Controllers.ActionFilters
{
    public abstract class ModelAccessorBaseActionFilterAttribute<Type> : ActionFilterAttribute
    where Type : class, IModel
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly System.Type _entityType;
        private protected IRepository<Type> DbSet;

        public ModelAccessorBaseActionFilterAttribute(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _entityType = typeof(Type);
            var dbSet = _unitOfWork.GetRepositoryOfType<Type>();

            if (dbSet is null)
            {
                throw new NotExistedDbSetOfProvidedEntityTypeException(_entityType);
            }

            DbSet = dbSet;
        }

        protected string EntityName => _entityType.Name;

        protected async Task CompleteDbWork()
        {
            await _unitOfWork.Complete();
        }

    }
}