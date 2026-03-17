using System.Reflection;

using Castle.Core.Internal;

using HiTechStore.Core.Exceptions;

namespace HiTechStore.Core.Services.Discount;

public interface IDiscountEntityResolver
{
    BaseDiscountEntityInterpreter GetDiscountEntityInterpreter(string entityPath);
}

/*
    The reason for using attribute approach for recognizing and resolving entity interpreters
    is that the defining and introducing interpreter get decoupled from manually discovering and
    registering like using Dictionary<string, BaseDiscountEntityInterpreter>.
*/
public class AttributeBaseDiscountEntityResolver : IDiscountEntityResolver
{
    static private Dictionary<string, Type> _interpretersCache = new();
    private IDiscountConditionValueComaprator _comaprator;
    public AttributeBaseDiscountEntityResolver(IDiscountConditionValueComaprator comaprator)
    {
        _comaprator = comaprator;
    }

    private BaseDiscountEntityInterpreter CreateInstance(Type entityInterpreterType)
    {
        var constructor = entityInterpreterType.GetConstructor([typeof(IDiscountConditionValueComaprator)]);

        if (constructor is null)
        {
            throw new NotFoundException($"Entity interpreter has no constructor with single parameter of type {nameof(IDiscountEntityResolverContext)}");
        }

        return (BaseDiscountEntityInterpreter)constructor.Invoke([_comaprator]);
    }

    public BaseDiscountEntityInterpreter GetDiscountEntityInterpreter(string entityPath)
    {

        if (_interpretersCache.TryGetValue(entityPath, out var interpreter))
        {
            return CreateInstance(interpreter);
        }

        var entityInterpreterType = Assembly.GetExecutingAssembly().GetTypes().Where(
            (type) => type.IsSubclassOf(typeof(BaseDiscountEntityInterpreter)) && type.GetAttribute<DiscountEntityMapAttribute>() is not null
        ).FirstOrDefault(
            (type) => string.Equals(type.GetAttribute<DiscountEntityMapAttribute>().EntityPath, entityPath, StringComparison.OrdinalIgnoreCase)
        );

        if (entityInterpreterType is null)
        {
            throw new NotFoundException($"Entity interpreter with path {entityPath} not found");
        }

        _interpretersCache.Add(entityPath, entityInterpreterType);

        return CreateInstance(entityInterpreterType);
    }
}


