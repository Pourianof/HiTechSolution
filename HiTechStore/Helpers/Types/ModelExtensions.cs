using HiTechStore.Core;
using HiTechStore.Models;

namespace HiTechStore.Helpers.Types
{
    public static class ModelExtensions
    {
        public static int? GetId(this IModel model)
        {
            var modelType = model.GetType();
            var modelTypeName = modelType.Name;

            var prop = modelType.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
                .FirstOrDefault(p => p.Name == "Id" || p.Name == $"{modelTypeName}Id")?.GetValue(model);


            return int.TryParse(prop?.ToString(), out var id) ? id : null;
        }
    }
}