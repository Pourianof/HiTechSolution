namespace HiTechStore.Models;

public class Brand
{
    public int BrandId { get; set; }
    public string? Name { get; set; }
    public virtual IEnumerable<BrandModel>? Models { get; set; }
}


public class BrandModel
{
    public int BrandModelId { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public virtual Brand? Brand { get; set; }
}