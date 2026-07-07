using HiTechStore.Infrastructure.Data.DTOs;
using HiTechStore.Infrastructure.Data.Queries;

namespace HiTechStore.Core.Common.Interfaces.Infra.Repositories
{
    public interface IRepositoryModelIndependent<TId>
        where TId : struct
    {
        Task<bool> IsExistsAsync(TId id);
        Task<bool> HasAnyAsync();
        Task<IEnumerable<ResourceExistenceResult<TId>>> CheckExistence(IEnumerable<TId> ids);
    }

    public interface IRepositoryModelBase<TModel, TId> : IRepositoryModelIndependent<TId>
        where TModel : class, IModel
        where TId : struct
    {
        Task<TModel?> GetModelByIdAsync(TId id);
        Task AddAsync(TModel entity);
        Task AddAllAsync(IEnumerable<TModel> entities);
        Task Delete(TModel entity);
        Task Delete(TId id);
        Task<int> DeleteImmediately(TId id);
        Task<IEnumerable<TModel>> GetAll(IEnumerable<TId> ids);
        Task<IEnumerable<TModel>> GetAllAsync();
        Task<IEnumerable<ResourceExistenceResultWithModel<TModel, TId>>> CheckExistence(IEnumerable<TId> ids, bool includeModel = false);
    }

    public interface IRepositoryBase<TModel, TProject, TId> : IRepositoryModelBase<TModel, TId>
        where TModel : class, IModel
        where TProject : class
        where TId : struct
    {
        Task<PagedResultDto<TProject>> GetPagedProjectedAsync(int limit = 10);
        Task<IEnumerable<TProject>> GetAllProjectedAsync();
        Task<TProject?> GetByIdProjectedAsync(TId id);
    }
    public interface IRepositoryBase<T, TId> : IRepositoryBase<T, T, TId>
       where T : class, IModel
        where TId : struct
    { }

    public interface IRepository<TModel, TProject, TQuery, TId> : IRepositoryBase<TModel, TProject, TId>
        where TModel : class, IModel
        where TQuery : BaseQuery
        where TProject : class
        where TId : struct
    {
        Task<PagedResultDto<TProject>> GetAllProjectedAsync(TQuery query);
        Task<PagedResultDto<TProjection>> GetAllProjectToAsync<TProjection>(TQuery? query = default);
        Task<TProjection?> GetByIdProjectTo<TProjection>(TId id);
        Task<TProjection?> GetByIdProjectTo<TProjection>(TId id, TQuery queryParams);
    }

    public interface IRepository<TModel, TProject, TId> : IRepository<TModel, TProject, BaseQuery, TId>
        where TModel : class, IModel
        where TProject : class
        where TId : struct
    { }

    public interface IRepository<TModel, TId> : IRepository<TModel, TModel, BaseQuery, TId>
        where TModel : class, IModel
        where TId : struct
    { }
    public interface IRepository<TModel> : IRepository<TModel, TModel, BaseQuery, int>
        where TModel : class, IModel
    { }
}
