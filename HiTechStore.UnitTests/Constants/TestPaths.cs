namespace HiTechStore.UnitTests.Constants;

public static class TestPaths
{
    public static readonly string Root = Directory.GetParent(AppContext.BaseDirectory)!.Parent!.Parent!.Parent!.FullName!;
    public static readonly string TestData = Path.Combine(Root, "TestData");
}
