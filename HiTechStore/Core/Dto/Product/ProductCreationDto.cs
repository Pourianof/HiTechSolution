
using HiTechStore.Core.Common.Interfaces.Infra;
using HiTechStore.Core.Dto.Product;

public class ProductCreationDto
{
    public string? Title { get; set; }
    public int? BrandModel { get; set; }
    public string? Description { get; set; }
    public ProductCategoryValuesDto? CategoryValues { get; set; }
    public IEnumerable<ProductVariationCreationDto>? Variations { get; set; }
    public IEnumerable<AppFile>? Media { get; set; }
    public IEnumerable<AppFile>? Thumbnails { get; set; }
}

public class ProductVariationCreationDto
{
    public double Price { get; set; }
    public int Color { get; set; }
    public int Inventory { get; set; }
    public IEnumerable<MediaMetaDataDto>? MediaMetaData { get; set; }
}

public class MediaMetaDataDto
{
    public bool IsMain { get; set; } = false;
    public int Index { get; set; }
    public int? ThumbnailIndex { get; set; }
}