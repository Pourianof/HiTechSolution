using System.Reflection;

using HiTechStore.Core;
using HiTechStore.Core.Repositories;

namespace HiTechStore.Helpers.Types
{
    public static class UnitOfWorkExtensions
    {
        public static IRepositoryBase<T>? GetRepositoryOfType<T>(this IUnitOfWork unitOfWork)
            where T : class, IModel
        {
            var modelType = typeof(T);
            var targetRepoType = typeof(IRepository<,>);

            var matchingProp = unitOfWork.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .FirstOrDefault(prop =>
                    ImplementsGenericInterface(prop.PropertyType, targetRepoType, modelType)
                );

            var prop = matchingProp?.GetValue(unitOfWork) as IRepositoryBase<T>;

            return prop;
        }

        private static bool ImplementsGenericInterface(Type concreteType, Type targetInterface, Type modelType)
        {
            return ClimbTypeTreeToFindType(concreteType, targetInterface, modelType);
        }

        private static bool ClimbTypeTreeToFindType(Type type, Type targetInterface, Type modelType)
        {
            var baseTypes = type.GetInterfaces().Append(type.BaseType);

            foreach (var t in baseTypes)
            {
                if (t is null)
                {
                    continue;
                }

                if (t.IsGenericType
                        && t.GetGenericTypeDefinition() == targetInterface
                        && t.GetGenericArguments().First() == modelType
                    )
                {
                    return true;
                }
                var iterateRes = ClimbTypeTreeToFindType(t, targetInterface, modelType);
                if (iterateRes)
                {
                    return true;
                }
            }

            return false;

        }
    }
}
