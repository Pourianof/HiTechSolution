using System.Linq.Expressions;

namespace HiTechStore.Helpers.URLFilterQuery.QueryAppliers;

public interface IQueryOperatorApplier<TModel>
{
    Expression<Func<TModel, bool>> ApplyOperator(object value, QueryOperator queryOperator);
}