using HiTechStore.Data.Queries;

namespace HiTechStore.Core.Repositories
{

    public interface IRepositoryModelIndependent
    {
        Task<bool> IsExistsAsync(int id);
    }
    public interface IRepositoryBase<T, O> : IRepositoryModelIndependent
        where T : class, IModel
        where O : class
    {
        Task<IEnumerable<O>> GetAllAsync();
        Task<O?> GetByIdAsync(int id);
        Task<T?> GetModelByIdAsync(int id);
        Task AddAsync(T entity);
        Task Delete(T entity);
        Task Delete(int id);
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
    }

    public interface IRepository<T, O> : IRepository<T, O, BaseQuery>
        where T : class, IModel
        where O : class
    { }

    public interface IRepository<T> : IRepository<T, T, BaseQuery>
        where T : class, IModel
    { }
}
