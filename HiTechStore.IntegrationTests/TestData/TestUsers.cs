namespace HiTechStore.IntegrationTests.TestData;

public class TestUsers
{
    public static readonly TestUser Admin = new TestUser("admin", "admin@hitechstore.com", "adminPassword123!");
    public static readonly TestUser Manager = new TestUser("manager", "manager@hitechstore.com", "managerPassword123!");
    public static readonly TestUser NormalUser = new TestUser("normaluser", "normal@x.com", "userPassword123!");
}

public record TestUser(
    string Username,
    string Email,
    string Password
);