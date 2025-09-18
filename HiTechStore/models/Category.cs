using HiTechStore.Core;

namespace HiTechStore.Models
{
    public class Category : IModel
    {
        public int CategoryId { get; set; }
        public int? ParentCategoryId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public virtual List<CategoryProperty>? CategoryProperties { get; set; }
    }

    public class CategoryProperty : IModel
    {
        public int CategoryPropertyId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public virtual Category? Category { get; set; }
    }

}