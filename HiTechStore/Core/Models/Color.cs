using HiTechStore.Core;

namespace HiTechStore.Core.Models;

public class Color : IModel
{
    public int ColorId { get; set; }
    public string? Code { get; set; }
    public string? Name { get; set; }
}