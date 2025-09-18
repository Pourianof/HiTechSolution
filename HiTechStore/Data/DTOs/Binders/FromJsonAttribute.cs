using System.Text.Json;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace HiTechStore.Data.DTOs.Binders;

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
            var valueAsString = valueProviderResult.FirstValue;
            var result = JsonSerializer.Deserialize(valueAsString!, bindingContext.ModelType);
            bindingContext.Result = ModelBindingResult.Success(result);
        }

        return Task.CompletedTask;
    }
}
