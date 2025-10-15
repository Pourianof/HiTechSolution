
using System.Diagnostics;

using HiTechStore.Core;
using HiTechStore.Core.Exceptions;

namespace HiTechStore.Helpers.IO;

internal enum CategoryAssetType
{
    Icon,
    Image
}

internal class CategoryAssetHelper
{
    public IFormFile? Icon { get; init; }
    public IFormFile? Image { get; init; }
    public bool DeleteOnError { get; set; }
    private IUnitOfWork _unitOfWork { get; }
    private int _categoryId { get; }
    public CategoryAssetHelper(IUnitOfWork unitOfWork, int categoryId)
    {
        _unitOfWork = unitOfWork;
        _categoryId = categoryId;
    }

    static string ProvideCategoryAssetPublicPath(int categoryId, CategoryAssetType type)
    {
        var basePath = Path.Combine("images", "category");
        return type switch
        {
            CategoryAssetType.Icon => Path.Combine(basePath, $"{categoryId}-icon.svg"),
            CategoryAssetType.Image => Path.Combine(basePath, $"{categoryId}.png"),
            _ => throw new UnreachableException()
        };
    }

    public static void RemoveAssets(int categoryId)
    {
        var imagePath = ProvideCategoryAssetPublicPath(categoryId, CategoryAssetType.Image);
        var iconPath = ProvideCategoryAssetPublicPath(categoryId, CategoryAssetType.Icon);

        if (imagePath is not null)
        {
            PublicAssetsHelper.DeleteFile(imagePath);
        }
        if (iconPath is not null)
        {
            PublicAssetsHelper.DeleteFile(iconPath);
        }
    }

    public static string? GetCategoryAssetPathIfExist(int categoryId, CategoryAssetType type)
    {
        var pubPath = ProvideCategoryAssetPublicPath(categoryId, type);
        return PublicAssetsHelper.IsExist(pubPath) ? pubPath : null;
    }

    public static string? GetCategoryImagePathIfExist(int categoryId)
    {
        return GetCategoryAssetPathIfExist(categoryId, CategoryAssetType.Image);
    }

    public static string? GetCategoryIconPathIfExist(int categoryId)
    {
        return GetCategoryAssetPathIfExist(categoryId, CategoryAssetType.Icon);
    }

    public async Task Write()
    {
        try
        {
            await WriteCategoryImage();
            await WriteCategoryIcon();
        }
        catch (SavingFileException)
        {
            if (DeleteOnError)
            {
                await _unitOfWork.Categories.Delete(_categoryId);
            }
            throw;
        }
    }


    private async Task WriteCategoryImage()
    {
        if (Image is not null)
        {
            var publicPath = ProvideCategoryAssetPublicPath(_categoryId, CategoryAssetType.Image);
            await PublicAssetsHelper.WriteIFormFile(Image, publicPath!);
        }
    }

    private async Task WriteCategoryIcon()
    {
        if (Icon is not null)
        {
            var publicPath = ProvideCategoryAssetPublicPath(_categoryId, CategoryAssetType.Icon);
            await PublicAssetsHelper.WriteIFormFile(Icon, publicPath);
        }
    }

    private string? GetAsset(CategoryAssetType type)
    {
        var assetPath = GetCategoryAssetPathIfExist(_categoryId, type);
        var (asset, name) = type switch { CategoryAssetType.Icon => (Icon, nameof(Icon)), _ => (Image, nameof(Icon)) };

        if (asset is null && assetPath is null)
        {
            throw new InvalidOperationException(
                $"{name} has not been registered before asking for its path"
            );
        }

        return assetPath;
    }

    public string? ImagePath
    {
        get => GetAsset(CategoryAssetType.Image);
    }

    public string? IconPath
    {
        get => GetAsset(CategoryAssetType.Icon);
    }

}