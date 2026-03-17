using HiTechStore.Models;

namespace HiTechStore.Core.Services.Discount;

public interface IDiscountConditionValueComaprator
{
    bool Compare(string criteriaValue, string conditionValue, DiscountOperation operation);
}


public class DiscountConditionValueComaprator : IDiscountConditionValueComaprator
{
    public bool Compare(string criteriaValue, string conditionValue, DiscountOperation operation)
    {
        if (operation == DiscountOperation.Equal)
        {
            return criteriaValue == conditionValue;
        }
        else
        {
            var num1 = int.Parse(criteriaValue);
            var num2 = int.Parse(conditionValue);

            switch (operation)
            {
                case DiscountOperation.GreaterThan: return num1 > num2;
                case DiscountOperation.GreaterThanOrEqual: return num1 >= num2;
                case DiscountOperation.LessThan: return num1 < num2;
                case DiscountOperation.LessThanOrEqual: return num1 <= num2;
            }
        }

        throw new NotImplementedException();
    }
}