using System.Reflection;

using AutoMapper;

namespace HiTechStore.Helpers.AutoMapper;

public static class AutoMapperExtension
{
    public static void RegisterAttributeMaps(this IMapperConfigurationExpression cfg, Assembly assembly)
    {
        var types = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract);

        foreach (var type in types)
        {
            var attributes = type.GetCustomAttributes<MapperAttribute>();

            foreach (var attr in attributes)
            {
                attr.Map(cfg, type);
            }
        }
    }
}
