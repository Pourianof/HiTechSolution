using System.Linq.Expressions;

namespace HiTechStore.Helpers.URLFilterQuery.QueryAppliers;

/**
    (TModel x) => x.ArrProp
    (TMiddle y) => y.Prop
    y.Prop => TReturn
**/
class SinglePropertyQueryApplier<TModel, TReturn> : QueryOperatorApplier<TModel>
{
    private Expression<Func<TModel, TReturn>> _target;

    public SinglePropertyQueryApplier(Expression<Func<TModel, TReturn>> target) : base(target.Parameters)
    {
        // it something like (x) => x.ArrProp
        _target = target;
    }

    protected override Expression Equal(object value)
    {
        return SinglePropertyQueryOperatorExpressionBuilder.Equal(_target, value);
    }

    protected override Expression GreaterThan(object value)
    {
        return SinglePropertyQueryOperatorExpressionBuilder.GreaterThan(_target, value);
    }

    protected override Expression GreaterThanOrEqual(object value)
    {
        return SinglePropertyQueryOperatorExpressionBuilder.GreaterThanOrEqual(_target, value);
    }

    protected override Expression In(object value)
    {
        return SinglePropertyQueryOperatorExpressionBuilder.In(_target, value);
    }

    protected override Expression Nin(object value)
    {
        return SinglePropertyQueryOperatorExpressionBuilder.Nin(_target, value);
    }

    protected override Expression LessThan(object value)
    {
        return SinglePropertyQueryOperatorExpressionBuilder.LessThan(_target, value);
    }

    protected override Expression LessThanOrEqual(object value)
    {
        return SinglePropertyQueryOperatorExpressionBuilder.LessThanOrEqual(_target, value);
    }
}