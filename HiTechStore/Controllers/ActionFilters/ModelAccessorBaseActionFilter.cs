using HiTechStore.Core;
using HiTechStore.Core.Repositories;
using HiTechStore.Helpers.Types;

using Microsoft.AspNetCore.Mvc.Filters;

namespace HiTechStore.Controllers.ActionFilters
{
    public abstract class ModelAccessorBaseActionFilterAttribute<TModel> : ActionFilterAttribute
    where TModel : class, IModel
    {

        protected readonly IUnitOfWork UnitOfWork;
        protected IRepositoryModelBase<TModel> Repo { get; set; }
        private readonly System.Type _entityType;


        public ModelAccessorBaseActionFilterAttribute(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
            _entityType = typeof(TModel);

            var repo = unitOfWork.GetRepositoryOfModelType<TModel>();

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