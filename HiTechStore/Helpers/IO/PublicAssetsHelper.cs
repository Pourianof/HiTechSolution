using HiTechStore.Core.Exceptions;

namespace HiTechStore.Helpers.IO;

public static class PublicAssetsHelper
{
    const string RootPath = "wwwroot";
    static public bool IsExist(string? publicPath)
    {
        return publicPath is not null && File.Exists(Path.Combine(RootPath, publicPath));
    }

    static public async Task WriteIFormFile(IFormFile file, string filePublicPath)
    {
        var filePath = Path.Combine("wwwroot", filePublicPath);
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
}