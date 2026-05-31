using HiTechStore.Infrastructure.Data.DTOs.DiscountEntity;
using HiTechStore.Helpers.AutoMapper;
using HiTechStore.Core.Models;

namespace HiTechStore.Infrastructure.Data.DTOs.Discount;

[MapFrom<Core.Models.Discount>]
public class DiscountDto
{
    public int DiscountId { get; set; }
    public string? Code { get; set; }
    public string? Description { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsDeactivated { get; set; }
    public string? CreatorId { get; set; }
    public ICollection<DiscountRuleDto>? Rules { get; set; }
}

[MapFrom<DiscountRule>]
public class DiscountRuleDto
{
    public int DiscountRuleId { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    // public ConditionComponentDto Condition { get; set; } = new();
    public DiscountActionDto? DiscountAction { get; set; }
    public string? RawConditionScript { get; set; }
}

[MapFrom<DiscountAction>]
public class DiscountActionDto
{
    public DiscountActionType Type { get; set; }
    public decimal Value { get; set; }
}


[MapFrom<ConditionComponent>]
public class ConditionComponentDto
{
    public int ConditionComponentId { get; set; }
    public string? Value { get; set; }
    public string? Type { get; set; }
    public DiscountEntityPropertyDto? Property { get; set; }
    public IEnumerable<ConditionComponentDto>? SubConditions { get; set; }
    public ConditionLambda? Method { get; set; }
}

[MapFrom<ConditionLambda>]
public class ConditionLambdaDto
{
    public int ConditionLambdaId { get; set; }
    public ConditionMethodDto? Method { get; set; }
    virtual public ConditionComponentDto? Body { get; set; }
}

[MapFrom<ConditionMethod>]
public class ConditionMethodDto
{
    public int ConditionMethodId { get; set; }
    public string? Name { get; set; }
    public DiscountEntityPropertyType ReturnType { get; set; }
}

