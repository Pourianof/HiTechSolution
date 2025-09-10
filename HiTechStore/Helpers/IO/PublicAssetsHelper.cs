namespace HiTechStore.Helpers.IO;

public static class PublicAssetsHelper
{
    const string RootPath = "wwwroot";
    static public bool IsExist(string? publicPath)
    {
        return publicPath is not null && File.Exists(Path.Combine(RootPath, publicPath));
    }
}