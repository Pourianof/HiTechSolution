using System.Collections;
using System.Linq.Expressions;

using HiTechStore.Helpers.Repository;
using HiTechStore.Helpers.URLFilterQuery;
using HiTechStore.Helpers.URLFilterQuery.QueryAppliers;





public static class FilterApplierHelper
{
    public static IQueryable<TModel> ApplyFiltersTo<TModel, TReturn>
        (this IQueryable<TModel> queryable, Dictionary<QueryOperator,
        OperatorValuePair> filters,
        IQueryOperatorApplier<TModel> queryOperatorApplier)
    {
        foreach (var (op, filter) in filters)
        {
            var value = op == QueryOperator.In || op == QueryOperator.Nin ?
                                filter.GetValue<IEnumerable<TReturn>>() :
                                (object?)filter.GetValue<TReturn>();
            if (value is null)
            {
                continue;
            }

            queryable = queryable.Where(queryOperatorApplier.ApplyOperator(value, op));
        }
        return queryable;
    }
}