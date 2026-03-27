using System.Collections;

using HiTechStore.Models;

namespace HiTechStore.Core.Services.Discount;

public interface IDiscountConditionValueComaprator
{
    // We can also include type of condition value or criteria value based-on 
    // [EntityProperty.Type].
    bool Compare(object criteriaValue, string conditionValue, DiscountOperation operation);
}


public class DiscountConditionValueComaprator : IDiscountConditionValueComaprator
{
    public bool Compare(object criteriaValue, string conditionValue, DiscountOperation operation)
    {
        if (operation == DiscountOperation.Equal)
        {
            return criteriaValue.ToString() == conditionValue;
        }
        else
        {
            var num1 = int.Parse(criteriaValue.ToString()!);
            var num2 = int.Parse(conditionValue);

            switch (operation)
            {
                case DiscountOperation.GreaterThan: return num1 > num2;
                case DiscountOperation.GreaterThanOrEqual: return num1 >= num2;
                case DiscountOperation.LessThan: return num1 < num2;
                case DiscountOperation.LessThanOrEqual: return num1 <= num2;
                case DiscountOperation.In:
                    {
                        if (criteriaValue is not IEnumerable)
                        {
                            throw new Exception(); // todo: appropriate error
                        }

                        var arr = (IEnumerable)criteriaValue;

                        foreach (var item in arr)
                        {
                            if (item.ToString() == conditionValue)
                            {
                                return true;
                            }
                        }

                        return false;
                    }
            }
        }

        throw new NotImplementedException();
    }
}