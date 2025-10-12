
using System.Linq.Expressions;

public static class ExpressionHelper
{
    public static string GetPropertyPath<T, TProperty>(Expression<Func<T, TProperty>> expression)
    {
        Expression body = expression.Body;

        // در صورت وجود تبدیل به object (مثل Convert(...))
        if (body is UnaryExpression unary && unary.Operand is MemberExpression)
            body = unary.Operand;

        var members = new List<string>();
        var memberExpr = body as MemberExpression;

        while (memberExpr != null)
        {
            members.Add(memberExpr.Member.Name);
            memberExpr = memberExpr.Expression as MemberExpression;
        }

        // چون از پایین به بالا جمع کردیم باید برعکس کنیم
        members.Reverse();

        return string.Join(".", members);
    }
}