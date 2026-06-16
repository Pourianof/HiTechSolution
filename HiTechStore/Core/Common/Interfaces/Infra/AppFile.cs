
namespace HiTechStore.Core.Common.Interfaces.Infra;

public class AppFile
{
    required public Stream File { get; set; }
    required public string FileName { get; set; }
    required public string ContentType { get; set; }
}