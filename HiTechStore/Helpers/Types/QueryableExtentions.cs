using System.Linq.Expressions;

namespace HiTechStore.Helpers.Types;

public static class QueryableExtentions
{
    static public IQueryable<T> FindById<T, TId>(this IQueryable<T> queryable, TId id)
    {
        var type = typeof(T);
        var idProperty = ModelHelper.GetModelIdPropertyInfo(type);
        var idPropertyName = idProperty?.Name;

        if (idPropertyName is null)
        {
            throw new Exception("Could not find model id");
        }

        // Expression for x => x.<Entity>Id == id
        var parameter = Expression.Parameter(type, "x");
        var propertyAccess = Expression.Property(parameter, idPropertyName);
        var constant = Expression.Constant(Convert.ChangeType(id, idProperty!.PropertyType));
        var equal = Expression.Equal(propertyAccess, constant);
        var lambda = Expression.Lambda<Func<T, bool>>(equal, parameter);

        return queryable.Where(lambda);
    }

    static public IQueryable<T> WhereIdExists<T, TId>(this IQueryable<T> queryable, IEnumerable<TId> ids)
    {
        var type = typeof(T);
        var idProperty = ModelHelper.GetModelIdPropertyInfo(type);
        var idPropertyName = idProperty?.Name;

        if (idPropertyName is null)
        {
            throw new Exception("Could not find model id");
        }

        var parameter = Expression.Parameter(type, "resource");
        var propertyAccess = Expression.Property(parameter, idPropertyName);

        var containsMethod = typeof(Enumerable)
                 .GetMethods()
                 .First(m => m.Name == nameof(Enumerable.Contains) && m.GetParameters().Length == 2)
                 .MakeGenericMethod(idProperty!.PropertyType);
        var containsMethodCall = Expression.Call(containsMethod, Expression.Constant(ids), propertyAccess);

        var lambda = Expression.Lambda<Func<T, bool>>(containsMethodCall, parameter);

        return queryable.Where(lambda);
    }
}