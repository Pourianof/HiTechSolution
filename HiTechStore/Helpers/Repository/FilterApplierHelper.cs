using System.Collections;
using System.Linq.Expressions;
using System.Reflection;

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
        // For "in" operator we recieve IEnumerable of values
        // so we try to figure out what is the elements type 
        var valueType = value.GetType();
        var enumerableType = typeof(IEnumerable);
        if (valueType.IsGenericType)
        {
            var genericType = valueType.GetGenericArguments().FirstOrDefault();
            if (genericType is null)
            {
                return Expression.Empty();
            }

            if (valueType.IsGenericType
                            && valueType.IsAssignableTo(enumerableType)
                        )
            {
                valueType = genericType;
                enumerableType = typeof(IEnumerable<>).MakeGenericType(valueType);
            }
        }

        var leftType = Nullable.GetUnderlyingType(targetParameter.Type) ??
                            targetParameter.Type ??
                            valueType;

        var left = Expression.Convert(
         targetParameter,
        leftType
        );

        Func<Expression> getInExpr = () =>
        {
            var containsMethod = typeof(Enumerable)
                .GetMethods()
                .First(m => m.Name == nameof(Enumerable.Contains) && m.GetParameters().Length == 2)
                .MakeGenericMethod(valueType);

            return Expression.Call(containsMethod, Expression.Constant(value), left);
        };

        var right = Expression.Constant(value);
        var ifNotNull = op switch
        {
            QueryOperator.Equal => Expression.Equal(left, right),
            QueryOperator.GreaterThan => Expression.GreaterThan(left, right),
            QueryOperator.LessThan => Expression.LessThan(left, right),
            QueryOperator.GreaterThanOrEqual => Expression.GreaterThanOrEqual(left, right),
            QueryOperator.LessThanOrEqual => Expression.LessThanOrEqual(left, right),
            QueryOperator.In => getInExpr(),
            _ => throw new NotSupportedException()
        };

        Expression notNullLeft = ifNotNull;
        if (leftType.IsAssignableTo(typeof(Nullable<>)))
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
            var value = op == QueryOperator.In ?
                                filter.GetValue<IEnumerable<TReturn>>() :
                                (object?)filter.GetValue<TReturn>();
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