using System.Collections;
using System.Linq.Expressions;

namespace HiTechStore.Helpers.URLFilterQuery.QueryAppliers;

public static class SinglePropertyQueryOperatorExpressionBuilder
{
    // build the proper left and right side of expression and return the correct lambda
    static private Expression ExpresionScaffoldBuilder(LambdaExpression selectorExpression, object value, Func<Expression, Expression, Expression> expressionBuilder)
    {
        // For "in" operator we recieve IEnumerable of values
        // so we try to figure out what is the elements type 
        var valueType = value.GetType();


        var targetParameter = selectorExpression.Body;

        var leftType = Nullable.GetUnderlyingType(targetParameter.Type) ??
                            targetParameter.Type ??
                            valueType;

        var left = Expression.Convert(
         targetParameter,
        leftType
        );

        var right = Expression.Constant(value);
        var ifNotNull = expressionBuilder(left, right);

        Expression notNullLeft = ifNotNull;

        if (leftType.IsAssignableTo(typeof(Nullable<>)))
        {
            notNullLeft = Expression.Condition(
                Expression.NotEqual(targetParameter, Expression.Constant(null)),
                ifNotNull,
                Expression.Constant(false)
                );
        }

        return Expression.Lambda(notNullLeft, selectorExpression.Parameters);
    }

    public static Expression Equal(LambdaExpression selectorExpression, object value)
    {
        return ExpresionScaffoldBuilder(selectorExpression, value, Expression.Equal);
    }

    public static Expression GreaterThan(LambdaExpression selectorExpression, object value)
    {
        return ExpresionScaffoldBuilder(selectorExpression, value, Expression.GreaterThan);
    }

    public static Expression GreaterThanOrEqual(LambdaExpression selectorExpression, object value)
    {
        return ExpresionScaffoldBuilder(selectorExpression, value, Expression.GreaterThanOrEqual);
    }

    private static Expression CollectionExpressionEval(LambdaExpression selectorExpression, object value, bool negate = false)
    {
        return ExpresionScaffoldBuilder(selectorExpression, value, (left, right) =>
        {
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

            var containsMethod = typeof(Enumerable)
                   .GetMethods()
                   .First(m => m.Name == nameof(Enumerable.Contains) && m.GetParameters().Length == 2)
                   .MakeGenericMethod(valueType);

            var containsInvokation = Expression.Call(containsMethod, Expression.Constant(value), left);

            if (negate)
            {
                return Expression.Not(containsInvokation);
            }

            return containsInvokation;
        });
    }

    public static Expression In(LambdaExpression selectorExpression, object value)
    {
        return CollectionExpressionEval(selectorExpression, value);
    }

    public static Expression Nin(LambdaExpression selectorExpression, object value)
    {
        return CollectionExpressionEval(selectorExpression, value, negate: true);
    }

    public static Expression LessThan(LambdaExpression selectorExpression, object value)
    {
        return ExpresionScaffoldBuilder(selectorExpression, value, Expression.LessThan);
    }

    public static Expression LessThanOrEqual(LambdaExpression selectorExpression, object value)
    {
        return ExpresionScaffoldBuilder(selectorExpression, value, Expression.LessThanOrEqual);
    }
}