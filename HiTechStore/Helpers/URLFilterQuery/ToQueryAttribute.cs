using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace HiTechStore.Helpers.URLFilterQuery;

[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
public class ToQueryAttribute : Attribute, IBindingSourceMetadata
{
    public BindingSource BindingSource => BindingSource.Query;
}

public class ToQueryModelBinder : IModelBinder
{
    private IQueryParser _queryParser;
    public ToQueryModelBinder(IQueryParser parser)
    {
        _queryParser = parser;
    }

    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        if (bindingContext == null) throw new ArgumentNullException(nameof(bindingContext));

        var query = bindingContext.HttpContext.Request.Query;

        var parsedQueries = _queryParser.Parse(query);
        var modelType = bindingContext.ModelType;

        var paramName = bindingContext.FieldName;

        try
        {
            var modelValue = parsedQueries.MapTo(modelType);
            bindingContext.Result = ModelBindingResult.Success(modelValue);
        }
        catch (Exception ex)
        {
            bindingContext.ModelState.TryAddModelError(paramName, ex.Message);
        }

        return Task.CompletedTask;
    }
}

public class ToQueryModelBinderProvider : IModelBinderProvider
{
    public IModelBinder GetBinder(ModelBinderProviderContext context)
    {
        if (context.BindingInfo.BindingSource != null &&
            context.BindingInfo.BindingSource.CanAcceptDataFrom(BindingSource.Query))
        {
            return new BinderTypeModelBinder(typeof(ToQueryModelBinder));
        }

        return null!;
    }
}