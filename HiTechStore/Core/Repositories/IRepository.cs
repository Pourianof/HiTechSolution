namespace HiTechStore.Core.Repositories
{
    public interface IRepository<T> where T : class, IModel
    {
        Task<IEnumerable<T>> GetAllAsync(int? Limit = 10);
        Task<T?> GetByIdAsync(int id);
        Task AddAsync(T entity);
        Task Delete(T entity);
        Task Delete(int id);
        Task<bool> IsExistsAsync(int id);
    }
}
