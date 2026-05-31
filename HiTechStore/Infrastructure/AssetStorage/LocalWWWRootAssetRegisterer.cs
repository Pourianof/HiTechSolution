using HiTechStore.Core.Common.Interfaces.Infra;
using HiTechStore.Core.Common.Interfaces.Presentation;
using HiTechStore.Core.Exceptions;
using HiTechStore.Infrastructure.Utils;
using HiTechStore.Core.Models;

public class LocalWWWRootAssetRegisterer(
    IApplicationContext applicationContext,
    IWellDistributedPathGenerator distributedPathGenerator
    ) : IPublicAssetRegisterer
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

    public async Task<string> WriteIFormFile(IFormFile file, WriteFileOptions options)
    {
        string filePath = "";
        if (options.WellDistributedPath)
        {
            var distPath = await distributedPathGenerator.Generate(file.FileName);

            filePath = Path.Combine(filePath, distPath);
        }

        if (options.PathParts?.Count() > 0)
        {
            var tempPath = "";
            foreach (var p in options.PathParts)
            {
                tempPath = Path.Combine(tempPath, p);
            }

            filePath = Path.Combine(tempPath, filePath);
        }

        var guid = Guid.NewGuid().ToString();

        filePath = Path.Combine(filePath, guid);
        filePath = Path.ChangeExtension(filePath, Path.GetExtension(file.FileName));

        await WriteIFormFile(file, filePath);

        return filePath;
    }
}