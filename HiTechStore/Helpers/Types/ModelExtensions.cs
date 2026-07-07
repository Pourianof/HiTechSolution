using System.Reflection;

using HiTechStore.Core;

namespace HiTechStore.Helpers.Types
{
    public static class ModelExtensions
    {
        public static TId? GetId<TId>(this IModel model)
            where TId : struct
        {
            var modelType = model.GetType();

            var prop = ModelHelper.GetModelIdPropertyInfo(modelType)?.GetValue(model);


            return prop as TId?;
        }
    }

    public static class ModelHelper
    {
        public static PropertyInfo? GetModelIdPropertyInfo(Type modelType)
        {
            var proxyStr = "Proxy";
            var modelTypeName = modelType.Name.EndsWith(proxyStr) ? modelType.Name.Substring(0, modelType.Name.Length - proxyStr.Length) : modelType.Name;

            return modelType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                 .FirstOrDefault(p => p.Name == "Id" || p.Name == $"{modelTypeName}Id");
        }

        public static string? GetModelIdPropertyName(Type modelType)
        {
            return GetModelIdPropertyInfo(modelType)?.Name; ;
        }
    }
}