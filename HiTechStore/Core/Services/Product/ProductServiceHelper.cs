using HiTechStore.Core.Common.Interfaces.Infra;
using HiTechStore.DTOs.Product;
using HiTechStore.Models;

public class ProductServiceHelper(
    IPublicAssetRegisterer assetRegisterer,
    IThumbnailGenerator thumbnailGenerator
    )
{
    public async Task<IEnumerable<VariationRegisteredMediaData>> RegisterCreatedProductMedia(int productId, ProductCreationDto productCreationDto)
    {

        List<VariationRegisteredMediaData> variationsMediaData = [];

        for (int variationIndex = 0; variationIndex < productCreationDto.Variations!.Count(); variationIndex++)
        {
            var variation = productCreationDto.Variations!.ElementAt(variationIndex);
            var variationMedia = variation.MediaMetaData!.Select(
                (meta) => new
                {
                    File = productCreationDto.Media!.ElementAt(meta.Index),
                    meta.IsMain,
                    Thumbnail = meta.ThumbnailIndex is not null ?
                        productCreationDto.Thumbnails?.ElementAt(meta.ThumbnailIndex.Value) :
                        default
                }
            );

            for (int index = 0; index < variationMedia.Count(); index++)
            {
                var media = variationMedia.ElementAt(index);
                var isImage = MediaTypeHelper.IsImage(media.File.FileName);

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

                variationsMediaData.Add(
                    new()
                    {
                        VariationIndex = variationIndex,
                        VariationMedia = productMedia
                    }
                );
            }
        }

        return variationsMediaData;
    }

}

public class VariationRegisteredMediaData
{
    public int VariationIndex { get; set; }
    public ProductMedia? VariationMedia { get; set; }
}