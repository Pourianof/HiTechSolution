using System.Linq.Expressions;

namespace HiTechStore.Helpers.Types;

public static class QueryableExtentions
{
    static public IQueryable<T> FindById<T>(this IQueryable<T> queryable, int id)
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
}