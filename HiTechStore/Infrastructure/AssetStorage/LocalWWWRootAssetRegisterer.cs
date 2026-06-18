using HiTechStore.Core.Common.Interfaces.Infra;
using HiTechStore.Core.Common.Interfaces.Presentation;
using HiTechStore.Core.Exceptions;
using HiTechStore.Infrastructure.Utils;

namespace HiTechStore.Infrastructure.AssetStorage;

public class LocalWWWRootAssetRegisterer : AssetRegistererBase
{
    private IApplicationContext _applicationContext;

    public LocalWWWRootAssetRegisterer(
        IApplicationContext applicationContext,
        IWellDistributedPathGenerator distributedPathGenerator
        ) : base(distributedPathGenerator)
    {
        _applicationContext = applicationContext;
    }

    override public bool IsExist(string? publicPath)
    {
        return publicPath is not null && File.Exists(Path.Combine(_applicationContext.GetAssetPath(), publicPath));
    }

    override public async Task SaveFileAsync(AppFile file, string filePublicPath)
    {
        var filePath = Path.Combine(_applicationContext.GetAssetPath(), filePublicPath);
        var dirPath = Path.GetDirectoryName(filePath);
        try
        {
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath!);
            }

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.File.CopyToAsync(stream);
            }
        }
        catch (Exception ex)
        {
            throw new SavingFileException("Problem with saving file", ex);
        }
    }

    override public void DeleteFile(string publicPath)
    {
        var filePath = Path.Combine(_applicationContext.GetAssetPath(), publicPath);
        File.Delete(filePath);
    }

    override public string GetPublicUrl(string relativePath)
    {
        var url = new UriBuilder(_applicationContext.GetServerPublicAccessUrl())
        {
            Path = $"{NormalizeUrl(relativePath).Trim('/')}"
        };

        return url.ToString();
    }

    override public async Task<string> SaveFileAsync(AppFile file, WriteFileOptions options)
    {
        var filePath = await ProvidePath(options, file.FileName);

        await SaveFileAsync(file, filePath);

        return filePath;
    }
}