using HiTechStore.Core;

using Microsoft.EntityFrameworkCore;

namespace HiTechStore.Core.Models;

[Index(nameof(NormalizedName), IsUnique = true)]
public class Brand : IModel
{
    public int BrandId { get; set; }
    public string? Name { get; set; }
    public string? NormalizedName
    {
        get => Name?.ToLower();
        set => Name?.ToLower();
    }
    public virtual List<BrandModel>? Models { get; set; }
}

public class BrandModel : IModel
{
    public int BrandModelId { get; set; }
    public string? Name { get; set; }
    public string? NormalizedName
    {
        get => Name?.ToLower();
        set => Name?.ToLower();
    }
    public string? Description { get; set; }
    public int BrandId { get; set; }
    public virtual Brand? Brand { get; set; }
}