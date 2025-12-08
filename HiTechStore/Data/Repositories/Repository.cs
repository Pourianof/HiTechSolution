using AutoMapper;
using AutoMapper.QueryableExtensions;

using HiTechStore.Core;
using HiTechStore.Core.Repositories;
using HiTechStore.Data.Queries;
using HiTechStore.Helpers.Types;
using HiTechStore.Helpers.URLFilterQuery;

using Microsoft.EntityFrameworkCore;

namespace HiTechStore.Data.Repositories
{
    public class RepositoryCore<TModel>(HiTechStoreDbContext context) : IRepositoryModelBase<TModel>
        where TModel : class, IModel
    {
        protected readonly HiTechStoreDbContext _context = context;
        protected readonly DbSet<TModel> _dbSet = context.Set<TModel>();
        public virtual async Task<TModel?> GetModelByIdAsync(int id)
        {
            var query = GetByIdAsyncQueryBuilder(_dbSet);
            return await query.FindById(id).FirstOrDefaultAsync();
        }

        protected virtual IQueryable<TModel> GetByIdAsyncQueryBuilder(IQueryable<TModel> queryBuilder)
        {
            return queryBuilder;
        }

        public virtual async Task AddAsync(TModel entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public virtual Task Delete(TModel entity)
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
            var modelType = typeof(TModel);
            var modelIdName = modelType.GetProperties().FirstOrDefault(p => p.Name.Contains("Id"))?.Name;
            if (modelIdName is null)
            {
                throw new InvalidOperationException("Entity does not have an Id property.");
            }
            return _dbSet.AnyAsync(e => EF.Property<int>(e, modelIdName) == id);
        }

        public async Task<IEnumerable<ResourceExistenceResult>> CheckExistence(IEnumerable<int> ids)
        {
            var existingResources = await _context.Set<TModel>()
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

        public async Task<IEnumerable<TModel>> GetAll(IEnumerable<int> ids)
        {
            return await _dbSet.WhereIdExists(ids).ToListAsync();
        }

        public async Task<IEnumerable<ResourceExistenceResultWithModel<TModel>>> CheckExistence(IEnumerable<int> ids, bool includeModel = false)
        {
            var existingResources = await _context.Set<TModel>()
               .WhereIdExists(ids)
               .ToListAsync();

            var existingIds = existingResources.Select((res) => res.GetId());

            return ids
                 .Select(id => new ResourceExistenceResultWithModel<TModel>
                 {
                     Id = id,
                     DoesExist = existingIds.Contains(id),
                     Model = existingResources.Where((res) => res.GetId() == id).FirstOrDefault()
                 })
                 .ToList();
        }
    }
    public class Repository<T, O, Q>(HiTechStoreDbContext context, IMapper mapper) :
        RepositoryCore<T>(context), IRepository<T, O, Q>
            where T : class, IModel
            where Q : BaseQuery
            where O : class
    {
        protected readonly IMapper _mapper = mapper;

        protected virtual IQueryable<T> GetAllQueryBuilder(IQueryable<T> queryBuilder, Q? queyParams = null)
        {
            return queryBuilder;
        }

        private IQueryable<T> BuildQueryBuilderBasedOnQueryParams(Q? queryParams)
        {
            var query = GetAllQueryBuilder(_dbSet.AsQueryable(), queryParams);

            if (queryParams?.SortBy is not null && queryParams.SortDir is not null)
            {
                var sortDir = queryParams.SortDir.GetValue<string>(QueryOperator.Equal)?.ToLower();
                if (sortDir == "des")
                {
                    query = query.OrderDescending();
                }
            }

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

            return query;
        }

        public async Task<IEnumerable<TProject>> GetAllProjected<TProject>(Q? queryParams)
        {
            return await Project<TProject>(BuildQueryBuilderBasedOnQueryParams(queryParams)).ToListAsync();
        }
        public virtual async Task<IEnumerable<O>> GetAllAsync(Q queryParams)
        {
            return await Project(BuildQueryBuilderBasedOnQueryParams(queryParams)).ToListAsync();
        }

        protected virtual IQueryable<TOut> Project<TOut>(IQueryable<T> queryable)
        {
            if (typeof(O) == typeof(T))
            {
                return (IQueryable<TOut>)queryable;
            }
            return queryable.ProjectTo<TOut>(_mapper.ConfigurationProvider);
        }

        protected virtual IQueryable<O> Project(IQueryable<T> queryable)
        {
            return Project<O>(queryable);
        }

        public virtual async Task<IEnumerable<O>> GetAllAsync()
        {
            return await Project(GetAllQueryBuilder(_dbSet.AsQueryable()).Take(10)).ToListAsync();
        }


        public virtual async Task<O?> GetByIdAsync(int id)
        {
            var query = GetByIdAsyncQueryBuilder(_dbSet);
            return await Project(query.FindById(id)).FirstOrDefaultAsync();
        }

        public Task<TProject?> GetByIdProjected<TProject>(int id)
        {
            var query = GetByIdAsyncQueryBuilder(_dbSet);
            return Project<TProject>(query.FindById(id)).FirstOrDefaultAsync();
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
