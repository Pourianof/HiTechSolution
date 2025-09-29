using Castle.Components.DictionaryAdapter.Xml;

using HiTechStore.Core.Exceptions;
using HiTechStore.Models;

namespace HiTechStore.Helpers.Types;

public static class PropertyValueModelExtension
{
    public static void PopulateValue(this PropertyValue propVal, PropertyType expectedType)
    {
        var value = propVal.ValueString;
        propVal.PopulateValue(expectedType, value);
    }
    public static void PopulateValue(this PropertyValue propVal, PropertyType expectedType, object? value)
    {
        try
        {
            var isString = false;
            switch (expectedType)
            {
                case PropertyType.Number:
                    propVal.ValueNumber = Convert.ToDouble(value); break;
                case PropertyType.String:
                    propVal.ValueString = Convert.ToString(value); isString = true; break;
                case PropertyType.Boolean:
                    propVal.ValueBoolean = Convert.ToBoolean(value); break;
                case PropertyType.DateTime:
                    propVal.ValueDateTime = Convert.ToDateTime(value); break;
                case PropertyType.Reference:
                    propVal.ValueReferenceId = Convert.ToInt16(value); break;
            }

            if (!isString)
            {
                propVal.ValueString = null;
            }
        }
        catch
        {
            throw new PropertyValueTypeDismatchException($"You need to provide a '{PropertyTypeHelper.GetNameOfCategoryPropertyType(expectedType)}' type value for specified property");
        }
    }
}