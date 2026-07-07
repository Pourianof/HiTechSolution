using HiTechStore.Core;
using HiTechStore.Core.Common.Interfaces.Infra.Repositories;
using HiTechStore.Helpers.Types;

using Microsoft.AspNetCore.Mvc.Filters;

namespace HiTechStore.Presentation.Controllers.ActionFilters
{
    public abstract class ModelAccessorBaseActionFilterAttribute<TModel, TId> : ActionFilterAttribute
    where TModel : class, IModel
    where TId : struct
    {

        protected readonly IUnitOfWork UnitOfWork;
        protected IRepositoryModelBase<TModel, TId> Repo { get; set; }
        private readonly Type _entityType;


        public ModelAccessorBaseActionFilterAttribute(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
            _entityType = typeof(TModel);

            var repo = unitOfWork.GetRepositoryOfModelType<TModel, TId>();

            if (repo is null)
            {
                throw new Exception($"No repository found for model of {EntityName}");
            }

            Repo = repo;

        }

        protected string EntityName => _entityType.Name;

        protected async Task CompleteDbWork()
        {
            await UnitOfWork.Complete();
        }

    }
}