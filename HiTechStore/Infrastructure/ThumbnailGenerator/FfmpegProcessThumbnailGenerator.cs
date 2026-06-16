
using System.Diagnostics;

using HiTechStore.Core.Common.Interfaces.Infra;


namespace HiTechStore.Infrastructure.ThumbnailGenerator;

public class FfmpegProcessThumbnailGenerator(ILogger<FfmpegProcessThumbnailGenerator> logger) : IThumbnailGenerator
{
    public async Task<Stream?> GenerateThumbnail(ThumbnailOptions thumbnailOptions)
    {

        string arguments = $"-ss {thumbnailOptions.CaptureTime.TotalSeconds}  -frames:v 1 -vf \"scale={thumbnailOptions.Width}:-1\"";

        var isStreamMode = true;

        if (!string.IsNullOrEmpty(thumbnailOptions.InputVideoPath))
        {
            arguments += $" -i \"{thumbnailOptions.InputVideoPath}\"";
            isStreamMode = false;
        }
        else if (thumbnailOptions.InputVideoStream is null)
        {
            throw new InvalidDataException("Neither input stream nor input video file path specified");
        }

        var processStartInfo = new ProcessStartInfo
        {
            FileName = "ffmpeg",
            Arguments = arguments + " pipe:1",
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

                if (isStreamMode)
                {
                    await thumbnailOptions.InputVideoStream!.CopyToAsync(process.StandardInput.BaseStream);
                    process.StandardInput.Close();
                }

                var outputStream = new MemoryStream();
                await process.StandardOutput.BaseStream.CopyToAsync(outputStream);
                outputStream.Position = 0;

                string errorOutput = await process.StandardError.ReadToEndAsync();
                await process.WaitForExitAsync();

                if (process.ExitCode == 0)
                {
                    return outputStream;
                }
                else
                {
                    logger.LogError("FFmpeg Error: {error}", errorOutput);
                    return default;
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError("Exception: {error}", ex);
            return default;

        }
    }
}