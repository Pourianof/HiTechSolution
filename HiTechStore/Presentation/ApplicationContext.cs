

using HiTechStore.Core.Common.Interfaces.Presentation;

namespace HiTechStore.Presentation;

public class ApplicationContext(
    IWebHostEnvironment environment,
    IConfiguration configuration
) : IApplicationContext
{
    public string GetAppRootPath()
    {
        return environment.ContentRootPath;
    }

    public string GetAssetPath()
    {
        return environment.WebRootPath;
    }

    public string GetServerPublicAccessUrl()
    {
        return configuration["PublicAccessUrl"]!;
    }
}