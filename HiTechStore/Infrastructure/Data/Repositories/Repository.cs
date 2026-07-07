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
    public class RepositoryCore<TModel, TId>(HiTechStoreDbContext context) : IRepositoryModelBase<TModel, TId>
        where TModel : class, IModel
        where TId : struct
    {
        protected readonly HiTechStoreDbContext _context = context;
        protected readonly DbSet<TModel> _dbSet = context.Set<TModel>();
        public virtual async Task<TModel?> GetModelByIdAsync(TId id)
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

        public virtual Task Delete(TId id)
        {
            var entity = _dbSet.Find(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
            }
            return Task.CompletedTask;
        }

        public virtual Task<int> DeleteImmediately(TId id)
        {
            return _dbSet.FindById(id).ExecuteDeleteAsync();
        }

        public virtual Task<bool> IsExistsAsync(TId id)
        {
            var modelType = typeof(TModel);
            var modelIdName = modelType.GetProperties().FirstOrDefault(p => p.Name.Contains("Id"))?.Name;
            if (modelIdName is null)
            {
                throw new InvalidOperationException("Entity does not have an Id property.");
            }
            return _dbSet.AnyAsync(e => EqualityComparer<TId>.Default.Equals(EF.Property<TId>(e, modelIdName), id));
        }

        public async Task<IEnumerable<ResourceExistenceResult<TId>>> CheckExistence(IEnumerable<TId> ids)
        {
            var existingResources = await _context.Set<TModel>()
               .WhereIdExists(ids)
               .ToListAsync();

            var existingIds = existingResources.Select((res) => res.GetId<TId>());

            return ids
                 .Select(id => new ResourceExistenceResult<TId>
                 {
                     Id = id,
                     DoesExist = existingIds.Contains(id)
                 })
                 .ToList();
        }

        public async Task<IEnumerable<TModel>> GetAll(IEnumerable<TId> ids)
        {
            return await _dbSet.WhereIdExists(ids).ToListAsync();
        }

        public async Task<IEnumerable<TModel>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<IEnumerable<ResourceExistenceResultWithModel<TModel, TId>>> CheckExistence(IEnumerable<TId> ids, bool includeModel = false)
        {
            var existingResources = await _context.Set<TModel>()
               .WhereIdExists(ids)
               .ToListAsync();

            var existingIds = existingResources
                .Select((res) => res.GetId<TId>())
                .Where((resId) => resId.HasValue)
                .Select((resId) => resId!.Value);

            return ids
                 .Select(id => new ResourceExistenceResultWithModel<TModel, TId>
                 {
                     Id = id,
                     DoesExist = existingIds.Contains(id),
                     Model = existingResources.FirstOrDefault(res =>
                         res.GetId<TId>() is TId resId && EqualityComparer<TId>.Default.Equals(resId, id))
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
    public class Repository<TModel, TProject, TQuery, TId>(HiTechStoreDbContext context, IMapper mapper) :
        RepositoryCore<TModel, TId>(context), IRepository<TModel, TProject, TQuery, TId>
            where TModel : class, IModel
            where TQuery : BaseQuery
            where TProject : class
            where TId : struct
    {
        protected readonly IMapper _mapper = mapper;

        protected virtual IQueryable<TModel> GetAllQueryBuilder(IQueryable<TModel> queryBuilder, TQuery? queyParams = null)
        {
            return queryBuilder;
        }



        private RepositoryHelper.QueryParamAppliedQuery<TModel> BuildQueryBuilderBasedOnQueryParams(IQueryable<TModel> baseQuery, TQuery? queryParams)
        {
            return RepositoryHelper.BuildQueryBuilderBasedOnQueryParams(
                GetAllQueryBuilder(baseQuery, queryParams),
                queryParams
            );
        }

        protected async Task<PagedResultDto<TOut>> GetPagedResult<TOut>(IQueryable<TModel> baseQuery, TQuery? queryParams)
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

        protected Task<PagedResultDto<TOut>> GetPagedResult<TOut>(TQuery? queryParams)
        {
            return GetPagedResult<TOut>(_dbSet.AsQueryable(), queryParams);
        }

        private Task<PagedResultDto<TOut>> BaseGetAll<TOut>(TQuery? queryParams)
        {
            return GetPagedResult<TOut>(queryParams);
        }

        public Task<PagedResultDto<TProjection>> GetAllProjectToAsync<TProjection>(TQuery? queryParams)
        {
            return GetPagedResult<TProjection>(queryParams);
        }
        public virtual Task<PagedResultDto<TProject>> GetAllProjectedAsync(TQuery queryParams)
        {
            return GetPagedResult<TProject>(queryParams);
        }

        protected virtual IQueryable<TProject> HandleProject(IQueryable<TModel> queryable, TQuery? queryParams = default)
        {
            return HandleProject<TProject>(queryable, queryParams);
        }

        protected virtual IQueryable<TOut> HandleProject<TOut>(IQueryable<TModel> queryable, TQuery? queryParams = default)
        {
            return queryable.ProjectTo<TOut>(_mapper.ConfigurationProvider);
        }

        protected IQueryable<TOut> Project<TOut>(IQueryable<TModel> queryable, TQuery? queryParams = default)
        {
            var outType = typeof(TOut);
            if (outType == typeof(TModel))
            {
                return (IQueryable<TOut>)queryable;
            }
            if (outType == typeof(TProject))
            {
                return (IQueryable<TOut>)HandleProject(queryable, queryParams);
            }

            return queryable.ProjectTo<TOut>(_mapper.ConfigurationProvider, queryParams);
        }

        protected IQueryable<TProject> Project(IQueryable<TModel> queryable, TQuery? queryParams = default)
        {
            return Project<TProject>(queryable, queryParams);
        }

        public async Task<IEnumerable<TProject>> GetAllProjectedAsync()
        {
            return await Project(GetAllQueryBuilder(_dbSet.AsQueryable())).ToListAsync();

        }

        public virtual async Task<PagedResultDto<TProject>> GetPagedProjectedAsync(int limit = 10)
        {
            var query = GetAllQueryBuilder(_dbSet.AsQueryable());
            var counts = await query.CountAsync();

            var projectedQuery = await Project(
                limit == 0 ?
                query :
                query.Take(limit)
            ).ToListAsync();

            return new PagedResultDto<TProject>()
            {
                Items = projectedQuery,
                PageNumber = 1,
                PageSize = limit,
                TotalCount = counts
            };
        }


        public virtual async Task<TProject?> GetByIdProjectedAsync(TId id)
        {
            var query = GetByIdAsyncQueryBuilder(_dbSet);
            return await Project(query.FindById(id)).FirstOrDefaultAsync();
        }

        public Task<TProjection?> GetByIdProjectTo<TProjection>(TId id)
        {
            var query = GetByIdAsyncQueryBuilder(_dbSet);
            return Project<TProjection>(query.FindById(id)).FirstOrDefaultAsync();
        }

        public Task<TProjection?> GetByIdProjectTo<TProjection>(TId id, TQuery queryParams)
        {
            var query = GetByIdAsyncQueryBuilder(_dbSet);
            return Project<TProjection>(query.FindById(id), queryParams).FirstOrDefaultAsync();
        }
    }

    public class Repository<TModel, TProject, TId> : Repository<TModel, TProject, BaseQuery, TId>
          where TModel : class, IModel
          where TProject : class
          where TId : struct
    {
        public Repository(HiTechStoreDbContext context, IMapper mapper)
            : base(context, mapper)
        {
        }
    }

    public class Repository<TModel, TId> : Repository<TModel, TModel, BaseQuery, TId>
            where TModel : class, IModel
            where TId : struct
    {
        public Repository(HiTechStoreDbContext context, IMapper mapper)
            : base(context, mapper)
        {
        }
    }

    public class Repository<TModel> : Repository<TModel, int>
            where TModel : class, IModel
    {
        public Repository(HiTechStoreDbContext context, IMapper mapper)
            : base(context, mapper)
        {
        }
    }
}