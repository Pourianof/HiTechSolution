using HiTechStore.Models;

namespace HiTechStore.Core.Helpers;

public interface IDiscountConditionParser
{
    ConditionComponent? Parse(string conditionString);
}
