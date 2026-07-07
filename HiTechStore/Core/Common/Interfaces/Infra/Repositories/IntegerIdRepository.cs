
using HiTechStore.Infrastructure.Data.Queries;

namespace HiTechStore.Core.Common.Interfaces.Infra.Repositories;

public interface IRepositoryModelIndependentWithIntegerId : IRepositoryModelIndependent<int>
{ }

public interface IRepositoryModelBaseWithIntegerId<TModel> : IRepositoryModelBase<TModel, int>
    where TModel : class, IModel
{ }

public interface IRepositoryBaseWithIntegerId<TModel, TProject> : IRepositoryBase<TModel, TProject, int>
    where TModel : class, IModel
    where TProject : class
{ }
public interface IRepositoryBaseWithIntegerId<TModel> : IRepositoryBase<TModel, TModel, int>
   where TModel : class, IModel
{ }

public interface IRepositoryWithIntegerId<TModel, TProject, TQuery> : IRepository<TModel, TProject, TQuery, int>
    where TModel : class, IModel
    where TQuery : BaseQuery
    where TProject : class
{ }

public interface IRepositoryWithIntegerId<TModel, TProject> : IRepository<TModel, TProject, BaseQuery, int>
    where TModel : class, IModel
    where TProject : class
{ }

public interface IRepositoryWithIntegerId<TModel> : IRepository<TModel, TModel, BaseQuery, int>
    where TModel : class, IModel
{ }