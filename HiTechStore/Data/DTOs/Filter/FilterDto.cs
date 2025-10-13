namespace HiTechStore.Data.DTOs;

public class FilterDto
{
    public IEnumerable<BrandFilterDto>? Brands { get; set; }
    public IEnumerable<FilterPropertyDto>? Properties { get; set; }
    public IEnumerable<FilterComponentsDto>? Components { get; set; }
    public ProductsPriceRangeDto? PriceRange { get; set; }
}

public class ProductsPriceRangeDto
{
    public double Max { get; set; }
    public double Min { get; set; }
}

public class BrandFilterDto
{
    public int BrandId { get; set; }
    public string? Name { get; set; }
    public int Frequency { get; set; }
}

public class FilterComponentsDto
{
    public int ComponentId { get; set; }
    public string? Name { get; set; }
    public IEnumerable<BrandFilterDto>? CommonBrands { get; set; }
    public IEnumerable<FilterPropertyDto>? Properties { get; set; }
}

public class FilterPropertyDto
{
    public int PropertyId { get; set; }
    public string? Name { get; set; }
    public string? Unit { get; set; }
    public int TotalFrequency { get; set; }
    public IEnumerable<PropertyCommomValueDto>? CommonValues { get; set; }
}


public class PropertyCommomValueDto
{
    public object? Value { get; set; }
    public int Frequency { get; set; }
}