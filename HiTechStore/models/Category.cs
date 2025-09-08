using HiTechStore.Core;

namespace HiTechStore.Models
{
    public class Category : IModel
    {
        public int CategoryId { get; set; }
        public int? ParentCategoryId { get; set; }
        public virtual Category? ParentCategory { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
    }
}