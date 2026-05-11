namespace HiTechStore.Core.Common.Interfaces.Infra;

public interface IPublicAssetRegisterer
{
    bool IsExist(string? publicPath);
    Task WriteIFormFile(IFormFile file, string filePublicPath);
    void DeleteFile(string publicPath);
    string GetAssetPhysicalFullPath(string relativePath);
}