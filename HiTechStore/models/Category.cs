using HiTechStore.Core;

namespace HiTechStore.Models
{
    public class Category : IModel
    {
        public int CategoryId { get; set; }
        public int? ParentCategoryId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public virtual List<Property>? Properties { get; set; }
        public virtual List<ComponentType>? Components { get; set; }
    }
}