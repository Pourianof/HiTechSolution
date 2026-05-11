
using HiTechStore.Core.Common.Interfaces.Infra;
using HiTechStore.Core.Common.Interfaces.Presentation;
using HiTechStore.Core.Exceptions;

public class LocalWWWRootAssetRegisterer(IApplicationContext applicationContext) : IPublicAssetRegisterer
{
    public bool IsExist(string? publicPath)
    {
        return publicPath is not null && File.Exists(Path.Combine(applicationContext.GetAssetPath(), publicPath));
    }

    public async Task WriteIFormFile(IFormFile file, string filePublicPath)
    {
        var filePath = Path.Combine(applicationContext.GetAssetPath(), filePublicPath);
        var dirPath = Path.GetDirectoryName(filePath);
        try
        {
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath!);
            }

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
        }
        catch (Exception ex)
        {
            throw new SavingFileException("Problem with saving file", ex);
        }
    }

    public void DeleteFile(string publicPath)
    {
        var filePath = Path.Combine(applicationContext.GetAssetPath(), publicPath);
        File.Delete(filePath);
    }

    public string GetAssetPhysicalFullPath(string relativePath)
    {
        return Path.Combine(applicationContext.GetAssetPath(), relativePath);
    }
}