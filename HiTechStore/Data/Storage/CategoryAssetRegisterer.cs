
using System.Diagnostics;

using HiTechStore.Core;
using HiTechStore.Core.Common.Interfaces.Infra;
using HiTechStore.Core.Exceptions;

namespace HiTechStore.Data.Storage;

public enum CategoryAssetType
{
    Icon,
    Image
}

public interface ICategoryAssetHelper
{
    IFormFile? Icon { get; set; }
    IFormFile? Image { get; set; }
    bool DeleteOnError { get; set; }

    string? GetCategoryAssetPathIfExist(int categoryId, CategoryAssetType type);
    string? GetCategoryIconPathIfExist(int categoryId);
    string? GetCategoryImagePathIfExist(int categoryId);
    string? GetIconPath(int cateGoryId);
    string? GetImagePath(int cateGoryId);
    void RemoveAssets(int categoryId);
    Task Write(int categoryId);
}

internal class CategoryAssetHelper : ICategoryAssetHelper
{
    public IFormFile? Icon { get; set; }
    public IFormFile? Image { get; set; }
    public bool DeleteOnError { get; set; }
    private IUnitOfWork _unitOfWork { get; }
    private IPublicAssetRegisterer AssetRegisterer { get; }
    public CategoryAssetHelper(IUnitOfWork unitOfWork, IPublicAssetRegisterer assetRegisterer)
    {
        _unitOfWork = unitOfWork;
        AssetRegisterer = assetRegisterer;
    }

    string ProvideCategoryAssetPublicPath(int categoryId, CategoryAssetType type)
    {
        var basePath = Path.Combine("images", "category");
        return type switch
        {
            CategoryAssetType.Icon => Path.Combine(basePath, $"{categoryId}-icon.svg"),
            CategoryAssetType.Image => Path.Combine(basePath, $"{categoryId}.png"),
            _ => throw new UnreachableException()
        };
    }

    public void RemoveAssets(int categoryId)
    {
        var imagePath = ProvideCategoryAssetPublicPath(categoryId, CategoryAssetType.Image);
        var iconPath = ProvideCategoryAssetPublicPath(categoryId, CategoryAssetType.Icon);

        if (imagePath is not null)
        {
            AssetRegisterer.DeleteFile(imagePath);
        }
        if (iconPath is not null)
        {
            AssetRegisterer.DeleteFile(iconPath);
        }
    }

    public string? GetCategoryAssetPathIfExist(int categoryId, CategoryAssetType type)
    {
        var pubPath = ProvideCategoryAssetPublicPath(categoryId, type);
        return AssetRegisterer.IsExist(pubPath) ? pubPath : null;
    }

    public string? GetCategoryImagePathIfExist(int categoryId)
    {
        return GetCategoryAssetPathIfExist(categoryId, CategoryAssetType.Image);
    }

    public string? GetCategoryIconPathIfExist(int categoryId)
    {
        return GetCategoryAssetPathIfExist(categoryId, CategoryAssetType.Icon);
    }

    public async Task Write(int categoryId)
    {
        try
        {
            await WriteCategoryImage(categoryId);
            await WriteCategoryIcon(categoryId);
        }
        catch (SavingFileException)
        {
            if (DeleteOnError)
            {
                await _unitOfWork.Categories.Delete(categoryId);
            }
            throw;
        }
    }


    private async Task WriteCategoryImage(int categoryId)
    {
        if (Image is not null)
        {
            var publicPath = ProvideCategoryAssetPublicPath(categoryId, CategoryAssetType.Image);
            await AssetRegisterer.WriteIFormFile(Image, publicPath!);
        }
    }

    private async Task WriteCategoryIcon(int categoryId)
    {
        if (Icon is not null)
        {
            var publicPath = ProvideCategoryAssetPublicPath(categoryId, CategoryAssetType.Icon);
            await AssetRegisterer.WriteIFormFile(Icon, publicPath);
        }
    }

    private string? GetAsset(CategoryAssetType type, int categoryId)
    {
        var assetPath = GetCategoryAssetPathIfExist(categoryId, type);
        var (asset, name) = type switch { CategoryAssetType.Icon => (Icon, nameof(Icon)), _ => (Image, nameof(Icon)) };

        if (asset is null && assetPath is null)
        {
            throw new InvalidOperationException(
                $"{name} has not been registered before asking for its path"
            );
        }

        return assetPath;
    }

    public string? GetImagePath(int cateGoryId) => GetAsset(CategoryAssetType.Image, cateGoryId);
    public string? GetIconPath(int cateGoryId) => GetAsset(CategoryAssetType.Icon, cateGoryId);

}