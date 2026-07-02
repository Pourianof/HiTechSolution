namespace HiTechStore.Core.Models;
// 00000000 -> 8th: scope bit (1 = all , 0 = self)
public class Permission : IModel
{
    public int Id { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
}


public static class Permissions
{
    public static class Product
    {
        public const string Create = "product:create";
        public const string Edit = "product:edit";
        public const string Delete = "product:delete";
    }

    public static class Comment
    {
        public const string Moderate = "comment:moderate";
    }

    public static class Discount
    {
        public const string Create = "discount:create";
        public const string View = "discount:list";
        public const string Edit = "discount:edit";
        public const string Delete = "discount:delete";
    }

    public static class Access
    {
        public const string Grant = "access:grant";
    }
}