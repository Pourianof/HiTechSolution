
using HiTechStore.Core.Common.Interfaces.Infra;
using HiTechStore.Infrastructure.Utils;

namespace HiTechStore.Infrastructure.AssetStorage;

abstract public class AssetRegistererBase : IPublicAssetRegisterer
{
    private IWellDistributedPathGenerator _wellDistributedPathGenerator;
    protected AssetRegistererBase(IWellDistributedPathGenerator distributedPathGenerator)
    {
        _wellDistributedPathGenerator = distributedPathGenerator;
    }
    public abstract void DeleteFile(string publicPath);
    public abstract string GetPublicUrl(string relativePath);
    public abstract bool IsExist(string? publicPath);
    public abstract Task SaveFileAsync(AppFile file, string filePublicPath);
    public abstract Task<string> SaveFileAsync(AppFile file, WriteFileOptions options);

    protected string NormalizeUrl(string url)
    {
        return url.Replace('\\', '/').TrimStart('/');
    }

    protected async Task<string> ProvidePath(WriteFileOptions options, string fileName)
    {
        string filePath = "";
        if (options.WellDistributedPath)
        {
            var distPath = await _wellDistributedPathGenerator.Generate(fileName);

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
        filePath = Path.ChangeExtension(filePath, Path.GetExtension(fileName));

        return filePath;
    }
}