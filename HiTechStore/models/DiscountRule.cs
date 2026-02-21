using HiTechStore.Core;

namespace HiTechStore.Models;

public class DiscountRule
{
    public int DiscountRuleId { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    virtual public List<DiscountConditionGroup> Conditions { get; set; } = new(); // condGroup1 OR condGroup2
    virtual public DiscountAction? DiscountAction { get; set; }
}

public class DiscountConditionGroup
{
    public int DiscountConditionGroupId { get; set; }
    public int DiscountRuleId { get; set; }

    virtual public ICollection<DiscountCondition>? Conditions { get; set; } // cond1 AND cond2
}

public class DiscountCondition
{
    public int DiscountConditionId { get; set; }
    public int EntityPropertyId { get; set; }
    virtual public DiscountEntityProperty? EntityProperty { get; set; }
    public int Priority { get; set; }
    public DiscountOperation Operation { get; set; }
    public string? Value { get; set; }
}

public enum DiscountOperation
{
    GreaterThan,
    GreaterThanOrEqual,
    LessThan,
    LessThanOrEqual,
    Equal,
    Contains
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