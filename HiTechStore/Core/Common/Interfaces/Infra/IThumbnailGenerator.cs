namespace HiTechStore.Core.Common.Interfaces.Infra;

public interface IThumbnailGenerator
{
    Task<Stream?> GenerateThumbnail(ThumbnailOptions thumbnailOptions);
}

public class ThumbnailOptions
{
    public string? InputVideoPath { get; set; }
    public Stream? InputVideoStream { get; set; }
    public TimeSpan CaptureTime { get; set; }
    public int Width { get; set; } = 320;
}