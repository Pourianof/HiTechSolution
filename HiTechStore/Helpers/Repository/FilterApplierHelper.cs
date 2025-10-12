using System.Linq.Expressions;

using HiTechStore.Helpers.URLFilterQuery;

namespace HiTechStore.Helpers.Repository;

public class FilterApplierHelper
{
    // Generate:
    // WHEN <left> != null THEN <left> <operator> <right> ELSE false END
    public static Expression CompareExpressionBuilder(
                    QueryOperator op,
                    object value,
                    Expression targetParameter
                    )
    {

        var left = Expression.Convert(
         targetParameter,
         value.GetType()
        );

        var right = Expression.Constant(value);
        var ifNotNull = op switch
        {
            QueryOperator.Equal => Expression.Equal(left, right),
            QueryOperator.GreaterThan => Expression.GreaterThan(left, right),
            QueryOperator.LessThan => Expression.LessThan(left, right),
            QueryOperator.GreaterThanOrEqual => Expression.GreaterThanOrEqual(left, right),
            QueryOperator.LessThanOrEqual => Expression.LessThanOrEqual(left, right),
            _ => throw new NotSupportedException()
        };

        Expression notNullLeft = ifNotNull;
        if (targetParameter.Type.IsAssignableTo(typeof(Nullable<>)))
        {
            notNullLeft = Expression.Condition(
                Expression.NotEqual(targetParameter, Expression.Constant(null)),
                ifNotNull,
                Expression.Constant(false)
                );
        }



        return notNullLeft;

    }

    public static IQueryable<TModel> ApplyFiltersTo<TModel, TReturn>(
        IQueryable<TModel> queryable,
        Expression<Func<TModel, TReturn>> to,
        Dictionary<QueryOperator, OperatorValuePair> filters)
    {
        foreach (var (op, filter) in filters)
        {
            var value = filter.GetValue(typeof(TReturn));
            if (value is null)
            {
                continue;
            }
            var body = FilterApplierHelper.CompareExpressionBuilder(op, value, to.Body);
            var compareExpression = Expression.Lambda<Func<TModel, bool>>(body, to.Parameters);
            queryable = queryable.Where(compareExpression);
        }
        return queryable;
    }
}