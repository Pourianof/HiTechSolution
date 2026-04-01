
using HiTechStore.Core;

namespace HiTechStore.Models;

public class DiscountEntity : IModel
{
    public int DiscountEntityId { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    virtual public ICollection<DiscountEntityProperty>? Properties { get; set; }
}

public class DiscountEntityProperty : IModel
{
    public int DiscountEntityPropertyId { get; set; }
    public string? Name { get; set; }
    public string? Path { get; set; }
    public string? Description { get; set; }
    public int EntityId { get; set; }
    virtual public DiscountEntity? Entity { get; set; }
    virtual public int? SubEntityId { get; set; }
    virtual public DiscountEntity? SubEntity { get; set; }
    public DiscountEntityPropertyType Type { get; set; }
}

public enum DiscountEntityPropertyType
{
    Boolean,
    Int,
    Float,
    String,
    Date,
    Array,
    Object
}