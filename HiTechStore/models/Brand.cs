using HiTechStore.Core;

namespace HiTechStore.Models;

public class Brand : IModel
{
    public int BrandId { get; set; }
    public string? Name { get; set; }
    public virtual List<BrandModel>? Models { get; set; }
}


public class BrandModel : IModel
{
    public int BrandModelId { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public virtual Brand? Brand { get; set; }
}