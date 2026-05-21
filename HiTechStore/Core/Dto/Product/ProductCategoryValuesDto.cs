namespace HiTechStore.Core.Dto.Product;

public class ProductCategoryValuesDto
{
    public int CategoryId { get; set; }
    public IEnumerable<PropertyValueEntryCreationDto>? Properties { get; set; }
    public IEnumerable<int>? ComponentModels { get; set; }
}

public class PropertyValueEntryCreationDto
{
    public int PropertyId { get; set; }
    public object? PropertyValue { get; set; }
}