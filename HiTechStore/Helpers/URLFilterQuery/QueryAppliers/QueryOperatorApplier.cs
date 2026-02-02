using System.Linq.Expressions;
namespace HiTechStore.Helpers.URLFilterQuery.QueryAppliers;

public abstract class QueryOperatorApplier<TModel> : IQueryOperatorApplier<TModel>
{
    private IEnumerable<ParameterExpression> _parameters;
    protected QueryOperatorApplier(IEnumerable<ParameterExpression> parameters)
    {
        _parameters = parameters;
    }
    public Expression<Func<TModel, bool>> ApplyOperator(object value, QueryOperator queryOperator) =>
        (Expression<Func<TModel, bool>>)(queryOperator switch
        {
            QueryOperator.Equal => Equal(value),
            QueryOperator.GreaterThan => GreaterThan(value),
            QueryOperator.LessThan => LessThan(value),
            QueryOperator.GreaterThanOrEqual => GreaterThanOrEqual(value),
            QueryOperator.LessThanOrEqual => LessThanOrEqual(value),
            QueryOperator.In => In(value),
            _ => throw new NotSupportedException()
        });


    protected virtual Expression Equal(object value) => Expression.Constant(true);


    protected virtual Expression GreaterThan(object value) => Expression.Constant(true);


    protected virtual Expression GreaterThanOrEqual(object value) => Expression.Constant(true);

    protected virtual Expression In(object value) => Expression.Constant(true);

    protected virtual Expression LessThan(object value) => Expression.Constant(true);

    protected virtual Expression LessThanOrEqual(object value) => Expression.Constant(true);
}