using HiTechStore.Infrastructure.Data.DTOs;
using HiTechStore.Infrastructure.Data.Queries;

namespace HiTechStore.Core.Common.Interfaces.Infra.Repositories
{

    public interface IRepositoryModelIndependent
    {
        Task<bool> IsExistsAsync(int id);
        Task<bool> HasAnyAsync();
        Task<IEnumerable<ResourceExistenceResult>> CheckExistence(IEnumerable<int> ids);
    }

    public interface IRepositoryModelBase<TModel> : IRepositoryModelIndependent
        where TModel : class, IModel
    {
        Task<TModel?> GetModelByIdAsync(int id);
        Task AddAsync(TModel entity);
        Task AddAllAsync(IEnumerable<TModel> entities);
        Task Delete(TModel entity);
        Task Delete(int id);
        Task<int> DeleteImmediately(int id);
        Task<IEnumerable<TModel>> GetAll(IEnumerable<int> ids);
        Task<IEnumerable<TModel>> GetAllAsync();
        Task<IEnumerable<ResourceExistenceResultWithModel<TModel>>> CheckExistence(IEnumerable<int> ids, bool includeModel = false);
    }

    public interface IRepositoryBase<T, O> : IRepositoryModelBase<T>
        where T : class, IModel
        where O : class
    {
        Task<PagedResultDto<O>> GetPagedProjectedAsync(int limit = 10);
        Task<IEnumerable<O>> GetAllProjectedAsync();
        Task<O?> GetByIdProjectedAsync(int id);
    }
    public interface IRepositoryBase<T> : IRepositoryBase<T, T>
       where T : class, IModel
    { }

    public interface IRepository<T, O, Q> : IRepositoryBase<T, O>
        where T : class, IModel
        where Q : BaseQuery
        where O : class
    {
        Task<PagedResultDto<O>> GetAllProjectedAsync(Q query);
        Task<PagedResultDto<TProject>> GetAllProjectToAsync<TProject>(Q? query = default);
        Task<TProject?> GetByIdProjectTo<TProject>(int id);
    }

    public interface IRepository<T, O> : IRepository<T, O, BaseQuery>
        where T : class, IModel
        where O : class
    { }

    public interface IRepository<T> : IRepository<T, T, BaseQuery>
        where T : class, IModel
    { }
}
