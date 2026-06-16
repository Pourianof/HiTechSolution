using HiTechStore.Core.Common.Interfaces.Infra;
using HiTechStore.Core.Models;

namespace HiTechStore.Infrastructure.AssetStorage;

public class ProductMediaRegisterer(
    IPublicAssetRegisterer assetRegisterer,
    IThumbnailGenerator thumbnailGenerator
)
{
    public async Task<ProductMedia> RegisterMedia(int productId, MediaData media)
    {
        var isImage = MediaTypeHelper.IsImage(media.Media.FileName);

        var productIdString = productId.ToString();

        var fileAccessPath = await assetRegisterer.SaveFileAsync(media.Media, new WriteFileOptions()
        {
            PathParts = isImage ? ["images", "products", productIdString] : ["videos", "products", productIdString]
        });

        var productMedia = new ProductMedia
        {
            FilePath = fileAccessPath,
            IsMain = media.IsMain,
            Type = MediaTypeHelper.GetMediaType(fileAccessPath)
        };

        if (!isImage)
        {
            string thumbnailPath;

            if (media.Thumbnail is not null)
            {
                // save thumbnail
                thumbnailPath = await assetRegisterer.SaveFileAsync(
                    media.Thumbnail, new WriteFileOptions()
                    {
                        PathParts = ["thumbnails", productIdString]
                    }
                );

                productMedia.ThumnailPath = thumbnailPath;
            }
            else
            {
                // create and save a thumbnail 
                var thumbnailStream = await thumbnailGenerator.GenerateThumbnail(
                    new()
                    {
                        InputVideoStream = media.Media.File,
                        CaptureTime = TimeSpan.FromMicroseconds(1)
                    }
                );

                if (thumbnailStream is not null)
                {
                    var thumbnailAccessPath = await assetRegisterer.SaveFileAsync(new()
                    {
                        File = thumbnailStream,
                        FileName = $"thumbnail-{media.Media.FileName}",
                        ContentType = media.Media.ContentType
                    }, new WriteFileOptions
                    {
                        PathParts = ["thumbnails", productIdString]
                    });

                    productMedia.ThumnailPath = thumbnailAccessPath;
                }
            }
        }

        return productMedia;
    }
}

public class MediaData
{
    required public AppFile Media { get; set; }
    public bool IsMain { get; set; }
    public AppFile? Thumbnail { get; set; }
}