using System.Reflection;

using HiTechStore.Core.Helpers;
using HiTechStore.Infrastructure.Data.Mapping.Discount;
using HiTechStore.Helpers.AutoMapper;
using HiTechStore.Helpers.ConditionParser;

namespace HiTechStore.Infrastructure.Data.Mapping;

public static class MappingDependenciesRegistration
{
    public static IServiceCollection AddMapping(this IServiceCollection services)
    {
        services.AddTransient<IRosylnDiscountConditionMapper, RoslynExpressionVisitorBase>();
        services.AddTransient<IDiscountConditionScriptParser, RoslynConditionScriptParser>();
        services.AddTransient<IConditionComponentTreeToLambdaExpression, ConditionComponentTreeToExpression>();
        services.AddTransient<ScriptToConditionComponentResolver>();

        services.AddAutoMapper((cfg) =>
        {
            cfg.RegisterAttributeMaps(Assembly.GetExecutingAssembly());
            cfg.AddProfile(typeof(MappingProfile));
        });

        return services;
    }
}