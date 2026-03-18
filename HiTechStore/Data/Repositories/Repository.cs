using AutoMapper;
using AutoMapper.QueryableExtensions;

using HiTechStore.Core;
using HiTechStore.Core.Repositories;
using HiTechStore.Data.DTOs;
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

        public async Task<IEnumerable<TModel>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
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

        public async Task AddAllAsync(IEnumerable<TModel> entities)
        {
            await _dbSet.AddRangeAsync(entities);
        }

        public Task<bool> HasAnyAsync()
        {
            return _dbSet.AnyAsync();
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

        private class QueryParamAppliedQuery
        {
            required public IQueryable<T>? BaseQuery { get; set; }
            required public IQueryable<T>? AppliedQuery { get; set; }
            required public int PageSize { get; set; }
            required public int Page { get; set; }
        }

        private QueryParamAppliedQuery BuildQueryBuilderBasedOnQueryParams(Q? queryParams)
        {
            var baseQuery = GetAllQueryBuilder(_dbSet.AsQueryable(), queryParams);
            var query = baseQuery;

            if (queryParams?.SortBy is not null && queryParams.SortDir is not null)
            {
                var sortDir = queryParams.SortDir.GetValue<string>(QueryOperator.Equal)?.ToLower();
                if (sortDir == "des")
                {
                    query = query.Reverse();
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

            return new()
            {
                BaseQuery = baseQuery,
                AppliedQuery = query,
                Page = page ?? 1,
                PageSize = limit ?? 0,
            };
        }

        private async Task<PagedResultDto<TOut>> BaseGetAll<TOut>(Q? queryParams)
        {
            var query = BuildQueryBuilderBasedOnQueryParams(queryParams);
            return new PagedResultDto<TOut>()
            {
                Items = await Project<TOut>(query.AppliedQuery!).ToListAsync(),
                PageNumber = query.Page,
                PageSize = query.PageSize,
                TotalCount = await query.BaseQuery!.CountAsync()
            };
        }

        public async Task<PagedResultDto<TProject>> GetAllProjectToAsync<TProject>(Q? queryParams)
        {
            var query = BuildQueryBuilderBasedOnQueryParams(queryParams);
            return new PagedResultDto<TProject>()
            {
                Items = await Project<TProject>(query.AppliedQuery!).ToListAsync(),
                PageNumber = query.Page,
                PageSize = query.PageSize,
                TotalCount = await query.BaseQuery!.CountAsync()
            };
        }
        public virtual async Task<PagedResultDto<O>> GetAllProjectedAsync(Q queryParams)
        {
            var query = BuildQueryBuilderBasedOnQueryParams(queryParams);
            return new PagedResultDto<O>()
            {
                Items = await Project(query.AppliedQuery!).ToListAsync(),
                PageNumber = query.Page,
                PageSize = query.PageSize,
                TotalCount = await query.BaseQuery!.CountAsync()
            };
        }

        protected virtual IQueryable<TOut> Project<TOut>(IQueryable<T> queryable)
        {
            if (typeof(TOut) == typeof(T))
            {
                return (IQueryable<TOut>)queryable;
            }
            return queryable.ProjectTo<TOut>(_mapper.ConfigurationProvider);
        }

        protected virtual IQueryable<O> Project(IQueryable<T> queryable)
        {
            return Project<O>(queryable);
        }

        public async Task<IEnumerable<O>> GetAllProjectedAsync()
        {
            return await Project(GetAllQueryBuilder(_dbSet.AsQueryable())).ToListAsync();

        }

        public virtual async Task<PagedResultDto<O>> GetPagedProjectedAsync(int limit = 10)
        {
            var query = GetAllQueryBuilder(_dbSet.AsQueryable());
            var counts = await query.CountAsync();

            var projectedQuery = await Project(
                limit == 0 ?
                query :
                query.Take(limit)
            ).ToListAsync();

            return new PagedResultDto<O>()
            {
                Items = projectedQuery,
                PageNumber = 1,
                PageSize = limit,
                TotalCount = counts
            };
        }


        public virtual async Task<O?> GetByIdProjectedAsync(int id)
        {
            var query = GetByIdAsyncQueryBuilder(_dbSet);
            return await Project(query.FindById(id)).FirstOrDefaultAsync();
        }

        public Task<TProject?> GetByIdProjectTo<TProject>(int id)
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
