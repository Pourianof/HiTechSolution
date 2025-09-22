using HiTechStore.Core;
using HiTechStore.Core.Repositories;
using HiTechStore.Data.Queries;
using HiTechStore.Helpers.Types;

using Microsoft.EntityFrameworkCore;

namespace HiTechStore.Data.Repositories
{
    public class Repository<T, Q> : IRepository<T, Q>
        where T : class, IModel
        where Q : BaseQuery
    {
        protected readonly HiTechStoreDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(HiTechStoreDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        protected virtual IQueryable<T> GetAllQueryBuilder(IQueryable<T> queryBuilder, Q? queyParams = null)
        {
            return queryBuilder;
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync(Q queryParams)
        {
            var query = GetAllQueryBuilder(_dbSet.AsQueryable(), queryParams);

            if (queryParams?.Page is not null)
            {
                query.Skip(
                    (queryParams.Limit ?? 0) * (queryParams.Page.Value - 1)
                );
            }

            if (queryParams?.Limit is not null)
            {
                query.Take(queryParams.Limit.Value);
            }

            return await query.ToListAsync();
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await GetAllQueryBuilder(_dbSet.AsQueryable()).Take(10).ToListAsync();
        }

        protected virtual IQueryable<T> GetByIdAsyncQueryBuilder(IQueryable<T> queryBuilder)
        {
            return queryBuilder;
        }
        public virtual async Task<T?> GetByIdAsync(int id)
        {
            var query = GetByIdAsyncQueryBuilder(_dbSet);
            return await query.FindById(id).FirstAsync();
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

    public class Repository<T> : Repository<T, BaseQuery>
            where T : class, IModel
    {
        public Repository(HiTechStoreDbContext context) : base(context)
        {
        }
    }
}
