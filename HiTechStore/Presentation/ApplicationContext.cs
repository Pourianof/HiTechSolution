

using HiTechStore.Core.Common.Interfaces.Presentation;

namespace HiTechStore.Presentation;

public class ApplicationContext(IWebHostEnvironment environment) : IApplicationContext
{
    public string GetAppRootPath()
    {
        return environment.ContentRootPath;
    }

    public string GetAssetPath()
    {
        return environment.WebRootPath;
    }
}