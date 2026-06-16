using HiTechStore.Core.Models;
using HiTechStore.Infrastructure.AssetStorage;

public class ProductServiceHelper(ProductMediaRegisterer productMediaRegisterer)
{
    public async Task<IEnumerable<VariationRegisteredMediaData>> RegisterCreatedProductMedia(int productId, ProductCreationDto productCreationDto)
    {

        List<VariationRegisteredMediaData> variationsMediaData = [];

        for (int variationIndex = 0; variationIndex < productCreationDto.Variations!.Count(); variationIndex++)
        {
            var variation = productCreationDto.Variations!.ElementAt(variationIndex);
            var variationMedia = variation.MediaMetaData!.Select(
                (meta) => new MediaData
                {
                    Media = productCreationDto.Media!.ElementAt(meta.Index),
                    IsMain = meta.IsMain,
                    Thumbnail = meta.ThumbnailIndex is not null ?
                        productCreationDto.Thumbnails?.ElementAt(meta.ThumbnailIndex.Value) :
                        default
                }
            );

            for (int index = 0; index < variationMedia.Count(); index++)
            {
                var media = variationMedia.ElementAt(index);

                var productMedia = await productMediaRegisterer.RegisterMedia(productId, media);

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