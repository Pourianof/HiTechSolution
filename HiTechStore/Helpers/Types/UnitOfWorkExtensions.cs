using System.Reflection;

using HiTechStore.Core;
using HiTechStore.Core.Repositories;

namespace HiTechStore.Helpers.Types
{
    public static class UnitOfWorkExtensions
    {
        public static IRepository<T>? GetRepositoryOfType<T>(this IUnitOfWork unitOfWork) where T : class, IModel
        {
            var targetRepoType = typeof(IRepository<>).MakeGenericType(typeof(T));

            var matchingProp = unitOfWork.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .FirstOrDefault(prop =>
                    ImplementsGenericInterface(prop.PropertyType, targetRepoType)
                );

            var prop = matchingProp?.GetValue(unitOfWork);

            return prop as IRepository<T>;
        }

        private static bool ImplementsGenericInterface(System.Type concreteType, System.Type targetInterface)
        {
            return concreteType
                .GetInterfaces()
                .Any(i => i.IsGenericType &&
                          i.GetGenericTypeDefinition() == typeof(IRepository<>) &&
                          i == targetInterface);
        }
    }
}