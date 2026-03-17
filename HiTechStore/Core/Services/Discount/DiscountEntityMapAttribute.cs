namespace HiTechStore.Core.Services.Discount;

[AttributeUsage(AttributeTargets.Class)]
public class DiscountEntityMapAttribute : Attribute
{
    public string? EntityPath { get; init; }
    public DiscountEntityMapAttribute(string entityPath)
    {
        EntityPath = entityPath;
    }

}
