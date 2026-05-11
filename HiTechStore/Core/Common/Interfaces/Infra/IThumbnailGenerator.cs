namespace HiTechStore.Core.Common.Interfaces.Infra;

public interface IThumbnailGenerator
{
    Task<bool> GenerateThumbnail(string videoPath, string thumbnailPath, TimeSpan captureTime, int width = 320);
}