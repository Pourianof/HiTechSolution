using HiTechStore.Data.Queries;

namespace HiTechStore.Core.Repositories
{

    public interface IRepositoryModelIndependent
    {
        Task<bool> IsExistsAsync(int id);
        Task<IEnumerable<ResourceExistenceResult>> CheckExistence(IEnumerable<int> ids);
    }

    public interface IRepositoryModelBase<TModel> : IRepositoryModelIndependent
        where TModel : class, IModel
    {
        Task<TModel?> GetModelByIdAsync(int id);
        Task AddAsync(TModel entity);
        Task Delete(TModel entity);
        Task Delete(int id);
        Task<int> DeleteImmediately(int id);
        Task<IEnumerable<TModel>> GetAll(IEnumerable<int> ids);
        Task<IEnumerable<ResourceExistenceResultWithModel<TModel>>> CheckExistence(IEnumerable<int> ids, bool includeModel = false);
    }

    public interface IRepositoryBase<T, O> : IRepositoryModelBase<T>
        where T : class, IModel
        where O : class
    {
        Task<IEnumerable<O>> GetAllAsync();
        Task<O?> GetByIdAsync(int id);
    }
    public interface IRepositoryBase<T> : IRepositoryBase<T, T>
       where T : class, IModel
    { }

    public interface IRepository<T, O, Q> : IRepositoryBase<T, O>
        where T : class, IModel
        where Q : BaseQuery
        where O : class
    {
        Task<IEnumerable<O>> GetAllAsync(Q query);
        Task<IEnumerable<TProject>> GetAllProjected<TProject>(Q? query = default);
        Task<TProject?> GetByIdProjected<TProject>(int id);
    }

    public interface IRepository<T, O> : IRepository<T, O, BaseQuery>
        where T : class, IModel
        where O : class
    { }

    public interface IRepository<T> : IRepository<T, T, BaseQuery>
        where T : class, IModel
    { }
}
