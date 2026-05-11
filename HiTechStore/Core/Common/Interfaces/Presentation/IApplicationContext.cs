namespace HiTechStore.Core.Common.Interfaces.Presentation;

/// <summary>
/// Define the context of launching app
/// </summary>
public interface IApplicationContext
{
    string GetAssetPath();
    string GetAppRootPath();
}