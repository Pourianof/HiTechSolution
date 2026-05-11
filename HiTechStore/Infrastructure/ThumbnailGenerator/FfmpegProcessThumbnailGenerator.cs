
using System.Diagnostics;

using HiTechStore.Core.Common.Interfaces.Infra;
using HiTechStore.Core.Common.Interfaces.Presentation;


namespace HiTechStore.Infrastructure.ThumbnailGenerator;

public class FfmpegProcessThumbnailGenerator() : IThumbnailGenerator
{
    public async Task<bool> GenerateThumbnail(string videoPath, string thumbnailPath, TimeSpan captureTime, int width = 320)
    {
        string arguments = $"-ss {captureTime.TotalSeconds} -i \"{videoPath}\" -frames:v 1 -vf \"scale={width}:-1\" \"{thumbnailPath}\"";

        var processStartInfo = new ProcessStartInfo
        {
            FileName = "ffmpeg",
            Arguments = arguments,
            UseShellExecute = false,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        try
        {
            using (var process = Process.Start(processStartInfo))
            {
                if (process is null)
                {
                    throw new Exception("Ffmpeg process could not launch");
                }

                string errorOutput = await process.StandardError.ReadToEndAsync();
                await process.WaitForExitAsync();

                if (process.ExitCode == 0 && File.Exists(thumbnailPath))
                {
                    return true;
                }
                else
                {
                    Console.WriteLine($"FFmpeg Error: {errorOutput}");
                    return false;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
            return false;
        }
    }
}