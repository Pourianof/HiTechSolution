using System.Reflection;

using HiTechStore.Core;

namespace HiTechStore.Helpers.Types
{
    public static class ModelExtensions
    {
        public static int? GetId(this IModel model)
        {
            var modelType = model.GetType();

            var prop = ModelHelper.GetModelIdPropertyInfo(modelType)?.GetValue(model);


            return int.TryParse(prop?.ToString(), out var id) ? id : null;
        }
    }

    public static class ModelHelper
    {
        public static PropertyInfo? GetModelIdPropertyInfo(Type modelType)
        {
            var modelTypeName = modelType.Name;

            return modelType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                 .FirstOrDefault(p => p.Name == "Id" || p.Name == $"{modelTypeName}Id");
        }

        public static string? GetModelIdPropertyName(Type modelType)
        {
            return GetModelIdPropertyInfo(modelType)?.Name; ;
        }
    }
}