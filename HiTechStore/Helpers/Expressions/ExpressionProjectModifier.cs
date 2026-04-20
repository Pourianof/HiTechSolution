using System.Linq.Expressions;

namespace HiTechStore.Helpers.Expressions;

public class ExpressionProjectModifier<T, TTarget> : ExpressionVisitor
{
    private readonly Dictionary<string, Expression> _propertyModifications;
    private readonly ParameterExpression _modelParameter;

    public ExpressionProjectModifier(Dictionary<string, Expression> propertyModifications, ParameterExpression modelParameter)
    {
        _propertyModifications = propertyModifications;
        _modelParameter = modelParameter;
    }

    protected override Expression VisitMemberInit(MemberInitExpression node)
    {
        var existingBindings = node.Bindings.ToList();
        var newBindings = new List<MemberBinding>();

        foreach (var binding in existingBindings)
        {
            var memberBinding = binding as MemberAssignment;
            if (memberBinding != null && _propertyModifications.ContainsKey(memberBinding.Member.Name))
            {
                var newValue = _propertyModifications[memberBinding.Member.Name];
                if (newValue is LambdaExpression lambda)
                {
                    newValue = ExpressionParameterReplacer.ReplaceParameter(
                        lambda, _modelParameter
                    );
                }
                newBindings.Add(Expression.Bind(memberBinding.Member, newValue));
            }
            else
            {
                newBindings.Add(binding);
            }

        }

        foreach (var newProp in _propertyModifications.Keys)
        {
            if (!existingBindings.Any(b => b.Member.Name == newProp))
            {
                var property = typeof(TTarget).GetProperty(newProp);
                if (property != null)
                {
                    newBindings.Add(Expression.Bind(property, _propertyModifications[newProp]));
                }
            }
        }

        return Expression.MemberInit(node.NewExpression, newBindings);
    }
}

public static class ProjectionExtensions
{
    public static Expression<Func<T, TTarget>> ModifyProjection<T, TTarget>(
        this Expression<Func<T, TTarget>> originalProjection,
        Dictionary<string, Expression> modifications)
    {
        var modifier = new ExpressionProjectModifier<T, TTarget>(modifications, originalProjection.Parameters.First());
        var newBody = modifier.Visit(originalProjection.Body);
        return Expression.Lambda<Func<T, TTarget>>(newBody, originalProjection.Parameters);
    }
}