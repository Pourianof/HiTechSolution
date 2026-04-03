using HiTechStore.Core.Helpers;
using HiTechStore.Helpers.ConditionParser;

namespace HiTechStore.Data.Mapping;

public static class MappingDependenciesRegistration
{
    public static IServiceCollection AddMapping(this IServiceCollection services)
    {
        services.AddTransient<IRosylnDiscountConditionMapper, RoslynExpressionVisitorBase>();
        services.AddTransient<IDiscountConditionScriptParser, RoslynConditionScriptParser>();
        services.AddTransient<IConditionComponentTreeToLambdaExpression, ConditionComponentTreeToExpression>();

        return services;
    }
}