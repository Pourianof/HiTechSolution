using HiTechStore.Infrastructure.Data.Queries;
using HiTechStore.Helpers.URLFilterQuery;

namespace HiTechStore.Infrastructure.Data.Repositories.Helpers;


public static class RepositoryHelper
{
    public static IQueryable<TModel> ApplyGenericQuery<TModel>(IQueryable<TModel> baseQuery, BaseQuery? queryParams)
    {
        if (queryParams is null)
        {
            return baseQuery;
        }

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

        return query;
    }

    public static QueryParamAppliedQuery<TModel> BuildQueryBuilderBasedOnQueryParams<TModel>(IQueryable<TModel> baseQuery, BaseQuery? queryParams)
    {
        var query = ApplyGenericQuery(baseQuery, queryParams);

        var page = queryParams?.Page?.GetValue<int>(QueryOperator.Equal);
        var limit = queryParams?.Limit?.GetValue<int>(QueryOperator.Equal);

        return new()
        {
            BaseQuery = baseQuery,
            AppliedQuery = query,
            Page = page ?? 1,
            PageSize = limit ?? 0,
        };
    }

    public class QueryParamAppliedQuery<TModel>
    {
        required public IQueryable<TModel>? BaseQuery { get; set; }
        required public IQueryable<TModel>? AppliedQuery { get; set; }
        required public int PageSize { get; set; }
        required public int Page { get; set; }
    }
}