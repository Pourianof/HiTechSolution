namespace HiTechStore.Core.Common.Interfaces.Infra;

public interface IPublicAssetRegisterer
{
    bool IsExist(string? publicPath);
    Task WriteIFormFile(IFormFile file, string filePublicPath);
    Task<string> WriteIFormFile(IFormFile file, WriteFileOptions options);
    void DeleteFile(string publicPath);
    string GetAssetPhysicalFullPath(string relativePath);
}

public class WriteFileOptions
{
    public IEnumerable<string>? PathParts { get; set; }
    public bool WellDistributedPath { get; set; } = false;
}