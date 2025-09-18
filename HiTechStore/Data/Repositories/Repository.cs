using HiTechStore.Core;
using HiTechStore.Core.Repositories;
using HiTechStore.Helpers.Types;

using Microsoft.EntityFrameworkCore;

namespace HiTechStore.Data.Repositories
{
    public class Repository<T> : IRepository<T> where T : class, IModel
    {
        protected readonly HiTechStoreDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(HiTechStoreDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        protected virtual IQueryable<T> GetAllQueryBuilder(IQueryable<T> queryBuilder)
        {
            return queryBuilder;
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync(int? Limit = 10)
        {
            return await GetAllQueryBuilder(_dbSet.AsQueryable()).Take(Limit!.Value).ToListAsync();
        }

        protected virtual IQueryable<T> GetByIdAsyncQueryBuilder(IQueryable<T> queryBuilder)
        {
            return queryBuilder;
        }
        public virtual async Task<T?> GetByIdAsync(int id)
        {
            return await GetByIdAsyncQueryBuilder(_dbSet).Where((entity) => entity.GetId() == id).FirstAsync();
        }

        public virtual async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public virtual Task Delete(T entity)
        {
            _dbSet.Remove(entity);
            return Task.CompletedTask;
        }

        public virtual Task Delete(int id)
        {
            var entity = _dbSet.Find(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
            }
            return Task.CompletedTask;
        }

        public virtual Task<bool> IsExistsAsync(int id)
        {
            var modelType = typeof(T);
            var modelIdName = modelType.GetProperties().FirstOrDefault(p => p.Name.Contains("Id"))?.Name;
            if (modelIdName is null)
            {
                throw new InvalidOperationException("Entity does not have an Id property.");
            }
            return _dbSet.AnyAsync(e => EF.Property<int>(e, modelIdName) == id);
        }
    }
}
