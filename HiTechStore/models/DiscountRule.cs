using HiTechStore.Core;

namespace HiTechStore.Models;

public class DiscountRule
{
    public int DiscountRuleId { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? ProductRawConditionScript { get; set; }
    public string? UserRawConditionScript { get; set; }
    virtual public ConditionComponent? ProductConditionTree { get; set; }
    virtual public ConditionComponent? UserConditionTree { get; set; }
    virtual public DiscountAction? DiscountAction { get; set; }
}

public enum DiscountConditionGroupType
{
    Cart,
    Product
}

public class ConditionComponent
{
    public int ConditionComponentId { get; set; }
    public string? Value { get; set; }
    virtual public ConditionComponentType? Type { get; set; }
    virtual public DiscountEntityProperty? Property { get; set; }
    public int? ParentId { get; set; }
    virtual public ConditionComponent? Parent { get; set; }
    virtual public IEnumerable<ConditionComponent>? SubConditions { get; set; }
    virtual public ConditionLambda? Lambda { get; set; }
}

public class ConditionLambda
{
    public int ConditionLambdaId { get; set; }
    virtual public ConditionMethod? Method { get; set; }
    public int OwnerConditionId { get; set; }
    virtual public ConditionComponent? OwnerCondition { get; set; }
    public int? BodyId { get; set; }
    virtual public ConditionComponent? Body { get; set; }
}

/*
    Any -> bool
    All -> bool
    Count -> number
*/
public class ConditionMethod : IModel
{
    public int ConditionMethodId { get; set; }
    public string? Name { get; set; }
    public DiscountEntityPropertyType ReturnType { get; set; }
}

public enum ConditionComponentType
{
    And,
    Or,
    GreaterThan,
    GreaterThanOrEqual,
    LessThan,
    LessThanOrEqual,
    Equality,
    NotEquality,
    Value,
    Not,
    Method,
}

public static class ConditionComponentTypeExtensions
{
    public static bool IsSizeComparator(this ConditionComponentType type)
    {
        return type == ConditionComponentType.GreaterThan &&
            type != ConditionComponentType.GreaterThanOrEqual &&
            type != ConditionComponentType.LessThan &&
            type != ConditionComponentType.LessThanOrEqual;
    }
}

public enum DiscountOperation
{
    GreaterThan,
    GreaterThanOrEqual,
    LessThan,
    LessThanOrEqual,
    Equal,
    Contains, // <multiple-value[]> Contains <single-value>
    In // <single value> In <multiple-value[]>
}

public class DiscountAction : IModel
{
    public DiscountActionType Type { get; set; } // Percent, Fixed
    public decimal Value { get; set; } // 10%
}


public enum DiscountActionType
{
    Percent,
    Fixed
}