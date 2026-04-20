
using System.Linq.Expressions;

namespace HiTechStore.Helpers.Expression;

using Expr = System.Linq.Expressions.Expression;

public class ExpressionParameterReplacer : ExpressionVisitor
{
    private readonly ParameterExpression _oldParameter;
    private readonly ParameterExpression _newParameter;

    public ExpressionParameterReplacer(ParameterExpression oldParameter, ParameterExpression newParameter)
    {
        _oldParameter = oldParameter;
        _newParameter = newParameter;
    }

    protected override Expr VisitParameter(ParameterExpression node)
    {
        return node == _oldParameter ? _newParameter : base.VisitParameter(node);
    }

    public static LambdaExpression ReplaceParameter(
    LambdaExpression expression,
    ParameterExpression newParameter)
    {
        var oldParam = expression.Parameters[0];

        var replacer = new ExpressionParameterReplacer(oldParam, newParameter);
        var newBody = replacer.Visit(expression.Body);

        return Expr.Lambda(newBody, newParameter);
    }
}