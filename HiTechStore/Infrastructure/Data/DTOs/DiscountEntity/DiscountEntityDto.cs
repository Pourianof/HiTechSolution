namespace HiTechStore.Infrastructure.Data.DTOs.DiscountEntity;

public class DiscountEntityDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public ICollection<DiscountEntityPropertyDto>? Properties { get; set; }
}

public class DiscountEntityLevel2Dto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public ICollection<DiscountEntityPropertyLevel2Dto>? Properties { get; set; }
}

public class DiscountEntityPropertyDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public DiscountEntityLevel2Dto? SubEntity { get; set; }
    public string? Type { get; set; }
}

public class DiscountEntityPropertyLevel2Dto : DiscountEntityPropertyDto
{

}