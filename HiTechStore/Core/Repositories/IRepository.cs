using HiTechStore.Data.Queries;

namespace HiTechStore.Core.Repositories
{
    public interface IRepositoryBase<T> where T : class, IModel
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id);
        Task AddAsync(T entity);
        Task Delete(T entity);
        Task Delete(int id);
        Task<bool> IsExistsAsync(int id);
    }
    public interface IRepository<T, Q> : IRepositoryBase<T>
        where T : class, IModel
        where Q : BaseQuery
    {
        Task<IEnumerable<T>> GetAllAsync(Q query);
    }

    public interface IRepository<T> : IRepository<T, BaseQuery>
        where T : class, IModel
    { }
}
