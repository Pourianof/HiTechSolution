using System.ComponentModel.DataAnnotations.Schema;

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
        public virtual List<CategoryComponent>? Components { get; set; }
    }

    public class CategoryComponent
    {
        public int CategoryComponentId { get; set; }
        public int? CategoryId { get; set; }
        public virtual Category? Category { get; set; }
        [ForeignKey("Component")]
        public int? ComponentId { get; set; }
        public virtual ComponentType? Component { get; set; }

    }
}