using HiTechStore.Data.DTOs.DiscountEntity;
using HiTechStore.Helpers.AutoMapper;
using HiTechStore.Models;

namespace HiTechStore.Data.DTOs.DiscountCode;

[MapFrom<Models.DiscountCode>]
public class DiscountCodeDto
{
    public string? Code { get; set; }
    public string? Description { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsDeactivated { get; set; }
    public ICollection<DiscountRuleDto>? Rules { get; set; }
}

[MapFrom<DiscountRule>]
public class DiscountRuleDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public List<DiscountConditionGroupDto> Conditions { get; set; } = new();
    public DiscountActionDto? DiscountAction { get; set; }
}

[MapFrom<DiscountAction>]
public class DiscountActionDto
{
    public DiscountActionType Type { get; set; }
    public decimal Value { get; set; }
}

[MapFrom<DiscountConditionGroup>]
public class DiscountConditionGroupDto
{
    public ICollection<DiscountConditionDto>? Conditions { get; set; }
}

[MapFrom<DiscountCondition>]
public class DiscountConditionDto
{
    public DiscountEntityPropertyDto? EntityProperty { get; set; }
    public int? Priority { get; set; }
    public DiscountOperation? Operation { get; set; }
    public string? Value { get; set; }
}