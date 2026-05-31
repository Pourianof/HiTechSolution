using HiTechStore.Core.Common.Interfaces.Infra;
using HiTechStore.Core.Models;

namespace HiTechStore.Infrastructure.Data.Storage;

public class ProductMediaRegisterer(
    IPublicAssetRegisterer assetRegisterer,
    IThumbnailGenerator thumbnailGenerator
)
{
    public async Task<ProductMedia> RegisterMedia(int productId, MediaData media)
    {
        var isImage = MediaTypeHelper.IsImage(media.File!.FileName);

        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(media.File.FileName);

        var productIdString = productId.ToString();

        string fileRelativePath = isImage ?
            Path.Combine("images", "products", productIdString, fileName) :
            Path.Combine("videos", "products", productIdString, fileName);

        await assetRegisterer.WriteIFormFile(media.File, fileRelativePath);

        var productMedia = new ProductMedia
        {
            FilePath = $"/{fileRelativePath}",
            IsMain = media.IsMain,
            Type = MediaTypeHelper.GetMediaType(fileRelativePath)
        };

        if (!isImage)
        {
            var thumbnailPath = Path.ChangeExtension(
                   Path.Combine("thumbnails", productIdString, Guid.NewGuid().ToString()),
                   MediaTypeHelper.Jpg
               );


            if (media.Thumbnail is not null)
            {
                // save thumbnail
                await assetRegisterer.WriteIFormFile(
                    media.Thumbnail, thumbnailPath
                );

                productMedia.ThumnailPath = $"/{thumbnailPath}";
            }
            else
            {
                var fullPath = assetRegisterer.GetAssetPhysicalFullPath(thumbnailPath);

                // create and save a thumbnail 
                var hasCreated = await thumbnailGenerator.GenerateThumbnail(
                    fileRelativePath,
                    fullPath,
                    TimeSpan.FromMicroseconds(1)
                );

                if (hasCreated)
                {
                    productMedia.ThumnailPath = $"/{thumbnailPath}";
                }
            }
        }

        return productMedia;
    }
}

public class MediaData
{
    public IFormFile? File { get; set; }
    public bool IsMain { get; set; }
    public IFormFile? Thumbnail { get; set; }
}