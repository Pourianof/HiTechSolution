namespace HiTechStore.Core.Models;

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

    public static class Access
    {
        public const string Grant = "access:grant";
    }
}