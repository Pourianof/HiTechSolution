namespace HiTechStore.IntegrationTests.Infrastructure;

public static class TestJwtTokenGenerator
{
    public static string Seperator = "__%__";
    public static string SchemePrefix = $"test{Seperator}";
    public static string GenerateTestJwtToken(string userId)
    {
        return string.Join(Seperator, ["test", userId]);
    }
}