namespace HiTechStore.Models
{
    public class IdentityRoles
    {
        public const string Admin = "Admin";
        public const string Manager = "Manager";
        public const string User = "User";

        public static readonly string[] AllRoles = { Admin, Manager, User };
    }
}