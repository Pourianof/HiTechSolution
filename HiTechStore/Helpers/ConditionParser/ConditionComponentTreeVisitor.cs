using HiTechStore.Models;

namespace HiTechStore.Helpers.ConditionParser;

public class ConditionComponentTreeVisitor<TReturn>
{
    virtual public TReturn? Visit(ConditionComponent condition)
    {
        switch (condition.Type)
        {
            case ConditionComponentType.And:
            case ConditionComponentType.Or:
            case ConditionComponentType.GreaterThan:
            case ConditionComponentType.GreaterThanOrEqual:
            case ConditionComponentType.LessThan:
            case ConditionComponentType.LessThanOrEqual:
            case ConditionComponentType.Equality:
            case ConditionComponentType.NotEquality:
                return BinaryOperator(condition);
            case ConditionComponentType.Method:
                return Method(condition);
            case ConditionComponentType.Value:
                return Value(condition);
            default:
                throw new NotSupportedException($"Visited condition component type not supported {Enum.GetName(typeof(ConditionComponentType), condition.Type!)}-{condition.Type}");
        }
    }

    virtual public TReturn? BinaryOperator(ConditionComponent condition)
    {
        return default;
    }

    virtual public TReturn? Method(ConditionComponent condition)
    {
        return default;
    }

    virtual public TReturn? Value(ConditionComponent condition)
    {
        return default;
    }
}

public class ConditionComponentTreeVisitor : ConditionComponentTreeVisitor<object>
{
}