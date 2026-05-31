using System.Text.Json;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace HiTechStore.Infrastructure.Data.DTOs.Binders;

public class FromJsonAttribute : ModelBinderAttribute
{
    public FromJsonAttribute() : base(typeof(JsonModelBinder)) { }
}

public class JsonModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

        if (valueProviderResult != ValueProviderResult.None)
        {
            try
            {
                var valueAsString = valueProviderResult.FirstValue;

                var result = JsonSerializer.Deserialize(valueAsString!, bindingContext.ModelType, new JsonSerializerOptions() { NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString });
                bindingContext.Result = ModelBindingResult.Success(result);
            }
            catch (JsonException ex)
            {
                bindingContext.ModelState.AddModelError($"{bindingContext.ModelName}{string.Join(".",
                    ex.Path?.Substring(1).Split(".").Select(
                    p => $"{char.ToUpper(p.First())}{p.Substring(1)}"
                    ) ?? []
                )}", ex.Message);
                bindingContext.Result = ModelBindingResult.Failed();
            }
        }

        return Task.CompletedTask;
    }
}
