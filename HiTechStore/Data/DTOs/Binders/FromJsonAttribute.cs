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
            try
            {
                var valueAsString = valueProviderResult.FirstValue;
                var result = JsonSerializer.Deserialize(valueAsString!, bindingContext.ModelType);
                bindingContext.Result = ModelBindingResult.Success(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                bindingContext.Result = ModelBindingResult.Failed();
            }
        }

        return Task.CompletedTask;
    }
}
