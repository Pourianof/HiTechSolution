using HiTechStore.Helpers.Types;

using Microsoft.Extensions.Primitives;

namespace HiTechStore.Helpers.URLFilterQuery;

public abstract class OperatorValuePair(QueryOperator @operator, StringValues value)
{
    public QueryOperator Operator { get; } = @operator;
    public StringValues Value { get; protected set; } = value;
    public TTarget? GetValue<TTarget>()
    {
        return QueryFilterItemHelper.Convert<TTarget>(Value);
    }

    public object? GetValue(Type targetType)
    {
        return QueryFilterItemHelper.Convert(Value, targetType);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Operator, Value);
    }

    public override bool Equals(object? obj)
    {
        return obj is OperatorValuePair p && p.Operator == Operator && p.Value == Value;
    }

    public abstract void HandleNewValue(StringValues value);
}

public abstract class ComparationBaseOperatorPair
    : OperatorValuePair
{
    public ComparationBaseOperatorPair(QueryOperator op, StringValues value)
        : base(op, value)
    {
        if (Value.Count() > 1)
        {
            Value = Value.Aggregate(
                  (a, b) =>
                  {
                      double? _a = QueryFilterItemHelper.Convert<double>(a);
                      double? _b = QueryFilterItemHelper.Convert<double>(b);
                      if (_a is null)
                      {
                          return b;
                      }
                      if (b is null)
                      {
                          return a;
                      }

                      return ShouldReplaceCompararation(_a.Value, _b.Value) ? b : a;
                  }
              );
        }
    }
    protected abstract bool ShouldReplaceCompararation(double oldVal, double newVal);

    public override void HandleNewValue(StringValues value)
    {
        double? currentVal = GetValue<double>();
        double? newVal = QueryFilterItemHelper.Convert<double>(Value);
        if (newVal is not null)
        {
            if (currentVal is null)
            {
                Value = value;
            }
            else if (ShouldReplaceCompararation(currentVal.Value, newVal.Value))
            {
                Value = value;
            }
        }
    }
}

public class GreaterThanOperatorPair(StringValues Value)
    : ComparationBaseOperatorPair(QueryOperator.GreaterThan, Value)
{
    protected override bool ShouldReplaceCompararation(double oldVal, double newVal)
    {
        return newVal > oldVal;
    }
}

public class GreaterThanOrEqualOperatorPair(StringValues Value)
    : ComparationBaseOperatorPair(QueryOperator.GreaterThanOrEqual, Value)
{
    protected override bool ShouldReplaceCompararation(double oldVal, double newVal)
    {
        return newVal > oldVal;
    }
}

public class LessThanOperatorPair(StringValues Value)
    : ComparationBaseOperatorPair(QueryOperator.LessThan, Value)
{
    protected override bool ShouldReplaceCompararation(double oldVal, double newVal)
    {
        return oldVal > newVal;
    }
}

public class LessThanOrEqualOperatorPair(StringValues Value)
    : ComparationBaseOperatorPair(QueryOperator.LessThanOrEqual, Value)
{
    protected override bool ShouldReplaceCompararation(double oldVal, double newVal)
    {
        return oldVal > newVal;
    }
}

public class EqualityOperatorPair
    : OperatorValuePair
{
    public EqualityOperatorPair(StringValues value) : base(QueryOperator.Equal, value)
    {
        // if multiple values defined for a key with equality operator
        // this we apply the last one
        if (Value.Count() > 1)
        {
            Value = Value.LastOrDefault();
        }
    }
    public override void HandleNewValue(StringValues value)
    {
        if (!string.IsNullOrEmpty(value) && value != Value)
        {
            Value = value;
        }
    }
}

public class CollectionBaseOperatorPair : OperatorValuePair
{
    public CollectionBaseOperatorPair(StringValues value, QueryOperator op) :
        base(op, value)
    {
        HandleValue(value, replace: true);
    }

    private void HandleValue(StringValues value, bool replace = false)
    {
        var newValues = value.WhereNotNull().SelectMany(v => v!.Split(','));
        if (!replace)
        {
            var valSet = Value.ToHashSet();
            valSet.UnionWith(newValues.ToHashSet());

            Value = new StringValues(valSet.WhereNotNull().ToArray());
        }
        else
        {
            Value = new StringValues(newValues.ToArray());
        }
    }

    public override void HandleNewValue(StringValues value)
    {
        HandleValue(value);
    }
}

public class InOperatorPair :
    CollectionBaseOperatorPair
{
    public InOperatorPair(StringValues value) : base(value, QueryOperator.In)
    {
    }
}

public class NotInOperatorPair :
    CollectionBaseOperatorPair
{
    public NotInOperatorPair(StringValues value) : base(value, QueryOperator.Nin)
    {
    }
}
