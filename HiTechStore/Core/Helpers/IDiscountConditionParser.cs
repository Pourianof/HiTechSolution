using HiTechStore.Core.Models;

namespace HiTechStore.Core.Helpers;

public interface IDiscountConditionScriptParser
{
    ConditionComponent? Parse(string conditionString);
}
