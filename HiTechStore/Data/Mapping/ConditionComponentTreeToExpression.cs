using System.Linq.Expressions;
using System.Reflection;

using HiTechStore.Helpers.ConditionParser;
using HiTechStore.Helpers.Types;
using HiTechStore.Models;

namespace HiTechStore.Data.Mapping;


public interface IConditionComponentTreeToLambdaExpression
{
    Expression<Func<TArg, bool>> Map<TArg>(ConditionComponent conditionComponent, string? wrappingLambdaPropertyName = default);
}


static class DiscountEntityPropertyTypeExtension
{
    public static Type GetCSharpType(this DiscountEntityPropertyType type)
    {
        return Map(
            type,
            onBoolean: () => typeof(bool),
            onInt: () => typeof(int),
            onFloat: () => typeof(double),
            onString: () => typeof(string),
            onDate: () => typeof(DateTime)
        );
    }

    public static TReturn Map<TReturn>(
        this DiscountEntityPropertyType type,
        Func<TReturn>? onBoolean = default,
        Func<TReturn>? onInt = default,
        Func<TReturn>? onFloat = default,
        Func<TReturn>? onString = default,
        Func<TReturn>? onDate = default,
        Func<TReturn>? callback = default
        )
    {
        return type switch
        {
            DiscountEntityPropertyType.Boolean => (onBoolean ?? callback)!.Invoke(),
            DiscountEntityPropertyType.Int => (onInt ?? callback)!.Invoke(),
            DiscountEntityPropertyType.Float => (onFloat ?? callback)!.Invoke(),
            DiscountEntityPropertyType.String => (onString ?? callback)!.Invoke(),
            DiscountEntityPropertyType.Date => (onDate ?? callback)!.Invoke(),
            _ => throw new NotSupportedException($"Evaluated type of operand for binary operator not supported {type}")
        };
    }
}

public class ConditionComponentTreeToExpression : ConditionComponentTreeVisitor<Expression>, IConditionComponentTreeToLambdaExpression
{
    protected Stack<ParameterExpression> WrappingLambdaParameterExpressionStack { get; set; } = new();
    public ConditionComponentTreeToExpression() { }
    protected ConditionComponentTreeToExpression(Stack<ParameterExpression> parameterExpressions)
    {
        WrappingLambdaParameterExpressionStack = parameterExpressions;
    }

    public override Expression? BinaryOperator(ConditionComponent condition)
    {
        if (condition.SubConditions?.Count() != 2)
        {
            throw new InvalidDataException("A binary operator condition have not enough operand");
        }

        var leftTypeResolver = new ExpressionTypeEvaluateHandler(WrappingLambdaParameterExpressionStack);
        var leftOperand = condition.SubConditions.First();
        var left = leftTypeResolver.Visit(leftOperand);

        var rightTypeResolver = new ExpressionTypeEvaluateHandler(WrappingLambdaParameterExpressionStack);
        var rightOperand = condition.SubConditions.ElementAt(1);
        var right = rightTypeResolver.Visit(rightOperand);

        // both member access -> ❌ convert
        // either member access -> ✔ convert to member access
        // none member access ->  ✔ convert based on operator

        if (leftTypeResolver.WasMemberAccess ^ rightTypeResolver.WasMemberAccess)
        {
            // one of them is member access
            var candidateType = leftTypeResolver.WasMemberAccess ? leftTypeResolver.CandidateType : rightTypeResolver.CandidateType;

            if (leftTypeResolver.WasMemberAccess)
            {
                right = new ContextualMemberAccessHandler(WrappingLambdaParameterExpressionStack, candidateType).Visit(rightOperand);
            }
            else
            {
                left = new ContextualMemberAccessHandler(WrappingLambdaParameterExpressionStack, candidateType).Visit(leftOperand);
            }
        }
        else if (!leftTypeResolver.WasMemberAccess && !rightTypeResolver.WasMemberAccess)
        {
            var isSizeComparationOperator = condition.Type!.Value.IsSizeComparator();

            if (isSizeComparationOperator)
            {
                // convert to number
                right = new ContextualMemberAccessHandler(WrappingLambdaParameterExpressionStack, DiscountEntityPropertyType.Float).Visit(rightOperand);
                left = new ContextualMemberAccessHandler(WrappingLambdaParameterExpressionStack, DiscountEntityPropertyType.Float).Visit(leftOperand);
            }
        }




        return condition.Type switch
        {
            ConditionComponentType.And => Expression.AndAlso(left!, right!),
            ConditionComponentType.Or => Expression.OrElse(left!, right!),
            ConditionComponentType.GreaterThan => Expression.GreaterThan(left!, right!),
            ConditionComponentType.GreaterThanOrEqual => Expression.GreaterThanOrEqual(left!, right!),
            ConditionComponentType.LessThan => Expression.LessThan(left!, right!),
            ConditionComponentType.LessThanOrEqual => Expression.LessThanOrEqual(left!, right!),
            ConditionComponentType.Equality => Expression.Equal(left!, right!),
            ConditionComponentType.NotEquality => Expression.NotEqual(left!, right!),
            _ => null,
        };
    }

    protected ParameterExpression? FindParameterWithName(string? name) =>
            WrappingLambdaParameterExpressionStack.LastOrDefault(p => p.Name == name);

    protected Type? FindTypeOfParameter(string name) =>
        FindParameterWithName(name)?.Type;

    protected Type? EvaluateEntityType(string entity, string? name = default)
    {
        if (name is not null)
        {
            var type = Expression.Property(FindParameterWithName(entity)!, name).Type;

            if (EnumberableHelpers.TryGetEnumerableItemType(type, out var genericType))
            {
                return genericType;
            }
        }
        else
        {
            return FindParameterWithName(entity)!.Type;
        }

        return default;

    }



    public override Expression? Method(ConditionComponent condition)
    {
        var body = condition.Lambda?.Body;
        var method = condition.Lambda!.Method;

        var wrappingLambdaParmeter = WrappingLambdaParameterExpressionStack!.Last();

        var parameterType = EvaluateEntityType(
            condition.Property!.Entity!.Name!,
            condition.Property!.Name
        ) ?? throw new InvalidCastException($"method called on an unknown entity type named {condition.Property.Name}");

        var lambdaParameter = Expression.Parameter(
            parameterType,
            condition.Property!.SubEntity!.Name
        );

        WrappingLambdaParameterExpressionStack.Push(lambdaParameter);
        var lambdaBodyExpr = body is not null ?
            new ContextualMemberAccessHandler(WrappingLambdaParameterExpressionStack).Visit(body)
        : null;
        WrappingLambdaParameterExpressionStack.Pop();

        MethodInfo callingMethod;
        Type enumberableType = typeof(Enumerable);
        if (body is null)
        {
            // empty method call
            callingMethod = enumberableType
                       .GetMethods()
                       .Where(m => string.Equals(m.Name, method!.Name, StringComparison.OrdinalIgnoreCase))
                       .First(m => m.GetParameters().Length == 1);
        }
        else
        {
            // method call with lambda argument
            callingMethod = enumberableType
            .GetMethods()
            .Where(m => string.Equals(m.Name, method!.Name, StringComparison.OrdinalIgnoreCase))
            .Where(m => m.GetParameters().Length == 2)
            // .Where(m => method.ReturnType == ConditionMethodReturnType.Bool ? typeof(bool) : method.ReturnType == ConditionMethodReturnType.Number ? typeof(int) : true)
            .First(m =>
            {
                var p = m.GetParameters()[1].ParameterType;
                return p.IsGenericType &&
                       p.GetGenericTypeDefinition() == typeof(Func<,>);
            });
        }

        if (callingMethod.GetGenericArguments().Length == 2)
        {

            callingMethod = callingMethod.MakeGenericMethod(parameterType, method!.ReturnType.GetCSharpType());
        }
        else
        {
            callingMethod = callingMethod.MakeGenericMethod(parameterType);

        }

        if (body is not null && lambdaBodyExpr is null)
        {
            throw new InvalidDataException("Could not convert condition's lambda body to proper expression");
        }

        var enumerableExtensionMethodFirstParameterType = typeof(IEnumerable<>).MakeGenericType(parameterType); // Method(IEnumerable<T>, lambda)
        var propertyMethodCalledOn =
                    Expression.Property(
                        FindParameterWithName(condition.Property.Entity!.Name)!,
                        condition.Property.Name!
                    );

        var methodCallExpression = lambdaBodyExpr is not null ? Expression.Call(
            callingMethod,
            propertyMethodCalledOn,
            Expression.Lambda(
                lambdaBodyExpr,
                lambdaParameter
            )
        ) : Expression.Call(callingMethod, propertyMethodCalledOn);

        return methodCallExpression;
    }

    public override Expression? Value(ConditionComponent condition)
    {
        return new ContextualMemberAccessHandler(
            WrappingLambdaParameterExpressionStack
        ).Visit(condition);
    }

    public Expression<Func<TArg, bool>> Map<TArg>(ConditionComponent conditionComponent, string? wrappingLambdaPropertyName = default)
    {
        var mainType = typeof(TArg);

        var finalFilteringLambdaParmeter = Expression.Parameter(mainType, wrappingLambdaPropertyName ?? mainType.Name);
        WrappingLambdaParameterExpressionStack.Push(finalFilteringLambdaParmeter);

        var innerExpression = Visit(conditionComponent);

        if (innerExpression is null)
        {
            throw new InvalidDataException("Could not convert specified condition component tree to Expression");
        }

        return Expression.Lambda<Func<TArg, bool>>(
            innerExpression,
            [finalFilteringLambdaParmeter]
        );
    }
}




class ExpressionTypeEvaluateHandler : ConditionComponentTreeToExpression
{
    public ExpressionTypeEvaluateHandler(Stack<ParameterExpression> parametersContext) : base(parametersContext) { }

    public DiscountEntityPropertyType CandidateType { get; private set; }
    /// <summary>
    /// It indicate the <c>CandidateType</c> is based on the Value type 
    /// or member access(<c>ConditionComponent.Property</c>)
    /// </summary>
    public bool WasMemberAccess { get; private set; } = false;
    public override Expression? BinaryOperator(ConditionComponent condition)
    {
        CandidateType = DiscountEntityPropertyType.Boolean;
        return base.BinaryOperator(condition);
    }

    public override Expression? Method(ConditionComponent condition)
    {
        var returnType = condition.Lambda?.Method?.ReturnType;
        CandidateType = returnType ?? DiscountEntityPropertyType.String;

        return base.Method(condition);
    }

    public override Expression? Value(ConditionComponent condition)
    {
        var memberAccessHandler = new ContextualMemberAccessHandler(
            WrappingLambdaParameterExpressionStack
        );

        var result = memberAccessHandler.Visit(condition);

        CandidateType = memberAccessHandler.WasMemberAccess ? condition.Property!.Type : DiscountEntityPropertyType.String;
        WasMemberAccess = memberAccessHandler.WasMemberAccess;

        return result;
    }
}

class ContextualMemberAccessHandler : ConditionComponentTreeToExpression
{
    private DiscountEntityPropertyType? _convertingType;
    public ContextualMemberAccessHandler(Stack<ParameterExpression> parametersContext) : base(parametersContext)
    { }

    public ContextualMemberAccessHandler(
        Stack<ParameterExpression> parametersContext,
        DiscountEntityPropertyType? convertingType) : this(parametersContext)
    {
        _convertingType = convertingType;
    }

    public bool WasMemberAccess { get; private set; }
    public override Expression? Value(ConditionComponent condition)
    {
        var targetParameter = FindParameterWithName(condition.Property?.Entity?.Name);
        if (condition.Value is null && targetParameter is not null)
        {
            // member access
            WasMemberAccess = true;
            return Expression.Property(
                targetParameter,
                condition.Property!.Name!
            );
        }
        else
        {
            object? value = condition.Value;
            if (_convertingType is not null)
            {
                if (_convertingType.Value == DiscountEntityPropertyType.Date)
                {
                    if (long.TryParse(condition.Value, out long ts))
                    {
                        return Expression.New(
                            typeof(DateTime).GetConstructor([typeof(long)])!,
                            Expression.Constant(ts)
                        );
                    }
                    else if (DateTime.TryParse(condition.Value, out var dt))
                    {
                        return Expression.Constant(dt);
                    }
                    else
                    {
                        throw new InvalidDataException("Could not convert a string value to datetime");
                    }

                }
                value = _convertingType.Value.Map(
                    callback: () => Convert.ChangeType(value, _convertingType.Value.GetCSharpType())
                );
            }

            return Expression.Constant(value);
        }
    }
}