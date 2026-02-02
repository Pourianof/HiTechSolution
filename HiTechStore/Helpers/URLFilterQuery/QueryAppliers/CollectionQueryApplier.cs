using System.Linq.Expressions;
namespace HiTechStore.Helpers.URLFilterQuery.QueryAppliers;

/**
    (TModel x) => x.ArrProp
    (TMiddle y) => y.Prop
    TReturn = typeof(y.Prop)
**/
class CollectionQueryApplier<TModel, TReturn, TMiddle> : QueryOperatorApplier<TModel>
{
    private Expression<Func<TModel, IEnumerable<TMiddle>>> _target;
    private Expression<Func<TMiddle, TReturn>> _propertySelector;

    public CollectionQueryApplier(Expression<Func<TModel, IEnumerable<TMiddle>>> target,
        Expression<Func<TMiddle, TReturn>> propertySelector) : base(target.Parameters)
    {
        // it something like (x) => x.ArrProp
        _target = target;
        // like: (y) => y.Prop
        _propertySelector = propertySelector;
    }
    /*
        x.Mehod(
            y => y.Prop
        )
    */
    private Expression<Func<TModel, bool>> GetQueryLambda(
        string methodName,
        LambdaExpression methodInvokationArgument,
        Func<Expression, Expression>? bodyGenerator = default)
    {
        // x
        var xParam = _target.Parameters[0];

        // x.ArrProp
        var arrPropBody = _target.Body;

        const int numberOfMethodArguments = 2; // Method<TModel>(this Enumerable<TModel> x, secondArg)
        var x = typeof(Enumerable)
            .GetMethods()
            .Where(m => m.Name == methodName)
            .Where(m => m.GetParameters().Length == numberOfMethodArguments)
            .First(m =>
            {
                var p = m.GetParameters()[1].ParameterType;
                return p.IsGenericType &&
                       p.GetGenericTypeDefinition() == typeof(Func<,>);
            });

        // find method Any<TSource, TResult>
        var method = typeof(Enumerable)
            .GetMethods()
            .Where(m => m.Name == methodName)
            .Where(m => m.GetParameters().Length == numberOfMethodArguments)
            .Where(m => m.ReturnType == methodInvokationArgument.ReturnType)
            .First(m =>
            {
                var p = m.GetParameters()[1].ParameterType;
                return p.IsGenericType &&
                       p.GetGenericTypeDefinition() == typeof(Func<,>);
            });

        if (method.GetGenericArguments().Length == 2)
        {

            method = method.MakeGenericMethod(methodInvokationArgument.Parameters.First().Type, methodInvokationArgument.ReturnType);
        }
        else
        {
            method = method.MakeGenericMethod(methodInvokationArgument.Parameters.First().Type);

        }


        var methodCallExpression = Expression.Call(
            method,
            arrPropBody,
            methodInvokationArgument
        );

        return Expression.Lambda<Func<TModel, bool>>(
            bodyGenerator is not null ? bodyGenerator(methodCallExpression) : methodCallExpression,
            xParam);
    }
    // generate
    // x.ArrProp.Any(y=> y.Prop == value)
    protected override Expression Equal(object value)
    {
        // y=> y.Prop == value
        var equalityCheckerLabmbda = Expression.Lambda(
                Expression.Equal(
                    _propertySelector.Body, Expression.Constant(value)
                ),
                _propertySelector.Parameters
            );

        return GetQueryLambda(nameof(Enumerable.Any), equalityCheckerLabmbda);
    }

    // generate
    // x.ArrProp.MinBy(y=> y.Prop) > value
    protected override Expression GreaterThan(object value)
    {
        return GetQueryLambda(nameof(Enumerable.Min),
            _propertySelector,
            (minExpr) => Expression.GreaterThan(minExpr, Expression.Constant(value)));
    }

    // generate
    // x.ArrProp.MinBy(y=> y.Prop) >= value
    protected override Expression GreaterThanOrEqual(object value)
    {
        return GetQueryLambda(
            nameof(Enumerable.Max),
            _propertySelector,
            (minExpr) => Expression.GreaterThanOrEqual(minExpr, Expression.Constant(value))
        );
    }

    // the goal is to convert: 
    // a) x=>x.ArrProp
    // b) y=> y.Prop
    // to:
    // x.ArrProp.Any(y=> [a,b,c,d].Contains(y))
    protected override Expression In(object values)
    {
        var containsMethod = typeof(Enumerable)
                .GetMethods()
                .First(m => m.Name == nameof(Enumerable.Contains) && m.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(TReturn));

        // [a,b,c,d].Contains(y)
        var containExpr = Expression.Call(containsMethod, Expression.Constant((object?)values), _propertySelector.Parameters[0]);

        return GetQueryLambda(nameof(Enumerable.Min), Expression.Lambda(
            containExpr, _propertySelector.Parameters[0]
        ));
    }

    // generate
    // x.ArrProp.MaxBy(y=> y.Prop) < value
    protected override Expression LessThan(object value)
    {
        return GetQueryLambda(nameof(Enumerable.Max),
            _propertySelector,
            (maxByExpr) => Expression.LessThan(maxByExpr, Expression.Constant(value)));
    }

    // generate
    // x.ArrProp.MaxBy(y=> y.Prop) <= value
    protected override Expression LessThanOrEqual(object value)
    {
        return GetQueryLambda(nameof(Enumerable.Max),
            _propertySelector,
            (maxByExpr) => Expression.LessThanOrEqual(maxByExpr, Expression.Constant(value)));
    }
}