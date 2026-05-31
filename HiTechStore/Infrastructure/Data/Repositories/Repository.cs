using AutoMapper;
using AutoMapper.QueryableExtensions;

using HiTechStore.Core;
using HiTechStore.Core.Common.Interfaces.Infra.Repositories;
using HiTechStore.Infrastructure.Data.DTOs;
using HiTechStore.Infrastructure.Data.Queries;
using HiTechStore.Infrastructure.Data.Repositories.Helpers;
using HiTechStore.Helpers.Types;

using Microsoft.EntityFrameworkCore;

namespace HiTechStore.Infrastructure.Data.Repositories
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



        private RepositoryHelper.QueryParamAppliedQuery<T> BuildQueryBuilderBasedOnQueryParams(IQueryable<T> baseQuery, Q? queryParams)
        {
            return RepositoryHelper.BuildQueryBuilderBasedOnQueryParams(
                GetAllQueryBuilder(baseQuery, queryParams),
                queryParams
            );
        }

        protected async Task<PagedResultDto<TOut>> GetPagedResult<TOut>(IQueryable<T> baseQuery, Q? queryParams)
        {
            var query = BuildQueryBuilderBasedOnQueryParams(baseQuery, queryParams);
            return new PagedResultDto<TOut>()
            {
                Items = await Project<TOut>(query.AppliedQuery!, queryParams).ToListAsync(),
                PageNumber = query.Page,
                PageSize = query.PageSize,
                TotalCount = await query.BaseQuery!.CountAsync()
            };
        }

        protected Task<PagedResultDto<TOut>> GetPagedResult<TOut>(Q? queryParams)
        {
            return GetPagedResult<TOut>(_dbSet.AsQueryable(), queryParams);
        }

        private Task<PagedResultDto<TOut>> BaseGetAll<TOut>(Q? queryParams)
        {
            return GetPagedResult<TOut>(queryParams);
        }

        public Task<PagedResultDto<TProject>> GetAllProjectToAsync<TProject>(Q? queryParams)
        {
            return GetPagedResult<TProject>(queryParams);
        }
        public virtual Task<PagedResultDto<O>> GetAllProjectedAsync(Q queryParams)
        {
            return GetPagedResult<O>(queryParams);
        }

        protected virtual IQueryable<O> HandleProject(IQueryable<T> queryable, Q? queryParams = default)
        {
            return HandleProject<O>(queryable, queryParams);
        }

        protected virtual IQueryable<TOut> HandleProject<TOut>(IQueryable<T> queryable, Q? queryParams = default)
        {
            return queryable.ProjectTo<TOut>(_mapper.ConfigurationProvider);
        }

        protected IQueryable<TOut> Project<TOut>(IQueryable<T> queryable, Q? queryParams = default)
        {
            var outType = typeof(TOut);
            if (outType == typeof(T))
            {
                return (IQueryable<TOut>)queryable;
            }
            if (outType == typeof(O))
            {
                return (IQueryable<TOut>)HandleProject(queryable, queryParams);
            }

            return queryable.ProjectTo<TOut>(_mapper.ConfigurationProvider, queryParams);
        }

        protected IQueryable<O> Project(IQueryable<T> queryable, Q? queryParams = default)
        {
            return Project<O>(queryable, queryParams);
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