using AutoMapper;
using AutoMapper.QueryableExtensions;

using HiTechStore.Core;
using HiTechStore.Core.Repositories;
using HiTechStore.Data.Queries;
using HiTechStore.Helpers.Types;
using HiTechStore.Helpers.URLFilterQuery;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace HiTechStore.Data.Repositories
{
    public class Repository<T, O, Q> : IRepository<T, O, Q>
        where T : class, IModel
        where Q : BaseQuery
        where O : class
    {
        protected readonly HiTechStoreDbContext _context;
        protected readonly DbSet<T> _dbSet;
        protected readonly IMapper _mapper;

        public Repository(HiTechStoreDbContext context, IMapper mapper)
        {
            _context = context;
            _dbSet = context.Set<T>();
            _mapper = mapper;
        }

        protected virtual IQueryable<T> GetAllQueryBuilder(IQueryable<T> queryBuilder, Q? queyParams = null)
        {
            return queryBuilder;
        }

        public virtual async Task<IEnumerable<O>> GetAllAsync(Q queryParams)
        {
            var query = GetAllQueryBuilder(_dbSet.AsQueryable(), queryParams);
            var page = queryParams?.Page?.GetValue<int>(QueryOperator.Equal);
            var limit = queryParams?.Limit?.GetValue<int>(QueryOperator.Equal);
            if (page is not null)
            {
                query = query.Skip(
                    (limit ?? 0) * (page.Value - 1)
                );
            }

            if (limit is not null)
            {
                query = query.Take(limit.Value);
            }

            return await Project(query).ToListAsync();
        }

        protected virtual IQueryable<O> Project(IQueryable<T> queryable)
        {
            if (typeof(O) == typeof(T))
            {
                return (IQueryable<O>)queryable;
            }
            return queryable.ProjectTo<O>(_mapper.ConfigurationProvider);
        }

        public virtual async Task<IEnumerable<O>> GetAllAsync()
        {
            return await Project(GetAllQueryBuilder(_dbSet.AsQueryable()).Take(10)).ToListAsync();
        }

        protected virtual IQueryable<T> GetByIdAsyncQueryBuilder(IQueryable<T> queryBuilder)
        {
            return queryBuilder;
        }
        public virtual async Task<O?> GetByIdAsync(int id)
        {
            var query = GetByIdAsyncQueryBuilder(_dbSet);
            return await Project(query.FindById(id)).FirstOrDefaultAsync();
        }

        public virtual async Task<T?> GetModelByIdAsync(int id)
        {
            var query = GetByIdAsyncQueryBuilder(_dbSet);
            return await query.FindById(id).FirstOrDefaultAsync();
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

        public virtual Task<int> DeleteImmediately(int id)
        {
            return _dbSet.FindById(id).ExecuteDeleteAsync();
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

        public async Task<IEnumerable<ResourceExistenceResult>> CheckExistence(IEnumerable<int> ids)
        {
            var existingResources = await _dbSet
                .WhereIdExists(ids)
                .ToListAsync();

            var existingIds = existingResources.Select((res) => res.GetId());

            return ids
                 .Select(id => new ResourceExistenceResult
                 {
                     Id = id,
                     DoesExist = existingIds.Contains(id)
                 })
                 .ToList();
        }
    }

    public class Repository<T, O> : Repository<T, O, BaseQuery>
          where T : class, IModel
          where O : class
    {
        public Repository(HiTechStoreDbContext context, IMapper mapper)
            : base(context, mapper)
        {
        }
    }

    public class Repository<T> : Repository<T, T, BaseQuery>
            where T : class, IModel
    {
        public Repository(HiTechStoreDbContext context, IMapper mapper)
            : base(context, mapper)
        {
        }
    }
}
