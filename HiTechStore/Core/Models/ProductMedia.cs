using HiTechStore.Core;

namespace HiTechStore.Core.Models;

public enum MediaType
{
    Image, Video
}

public class ProductMedia : IModel
{
    public int ProductMediaId { get; set; }
    public bool IsMain { get; set; }
    public string? FilePath { get; set; }
    public MediaType Type { get; set; }
    public int ProductId { get; set; }
    public string? ThumnailPath { get; set; }
}



public static class MediaTypeHelper
{
    static public string Jpg = "jpg";
    static public string Jpeg = "jpeg";
    static public string Png = "png";
    static public string Mp4 = "mp4";

    static readonly string[] ValidMediaTypes = [Jpg, Jpeg, Png, Mp4];
    public static string[] ValidTypes()
    {
        return ValidMediaTypes;
    }

    public static bool IsValid(string filePath)
    {
        return ValidMediaTypes.Any((type) => filePath.ToLower().EndsWith($".{type}"));
    }

    public static string[] GetValidImageTypes()
    {
        return ValidMediaTypes.Where((type) => type != "mp4").ToArray();
    }

    public static bool IsImage(string filePath)
    {
        return GetValidImageTypes().Any((type) => filePath.ToLower().EndsWith($".{type}"));
    }

    public static MediaType GetMediaType(string filePath)
    {
        return IsImage(filePath) ? MediaType.Image : MediaType.Video;
    }
    public static string GetMediaTypeName(MediaType mediaType)
    {
        return mediaType switch { MediaType.Image => "Image", MediaType.Video => "Video", _ => throw new NotImplementedException() };
    }
}