using HiTechStore.Core;
using HiTechStore.Core.Common.Interfaces.Infra.Repositories;

namespace HiTechStore.Helpers.Types;

public static class UnitOfWorkExtensions
{
    public static T? GetRepositoryOfType<T>(this IUnitOfWork unitOfWork)
        where T : IRepositoryModelIndependent
    {
        var repoType = typeof(T);

        var props = unitOfWork.GetType().GetProperties();

        return (T?)props.Where(
            (prop) => prop.PropertyType == repoType
        ).FirstOrDefault()?.GetValue(unitOfWork);
    }

    public static IRepositoryModelBase<TModel>? GetRepositoryOfModelType<TModel>(this IUnitOfWork unitOfWork)
        where TModel : class, IModel
    {
        var targetType = unitOfWork.GetType();
        var modelType = typeof(TModel);

        foreach (var prop in targetType.GetProperties())
        {
            var propType = prop.PropertyType;

            var interfaces = propType.GetInterfaces()
                .Where(i => i.IsGenericType)
                .Select(i => new { GenericDefinition = i.GetGenericTypeDefinition(), FirstArg = i.GetGenericArguments().FirstOrDefault() });

            if (interfaces.Any(i => (
                i.GenericDefinition == typeof(IRepository<,,>) ||
                i.GenericDefinition == typeof(IRepository<,>) ||
                i.GenericDefinition == typeof(IRepository<>)) && i.FirstArg == modelType))

            {
                return prop.GetValue(unitOfWork) as IRepositoryModelBase<TModel>;
            }
        }

        return null;
    }
}