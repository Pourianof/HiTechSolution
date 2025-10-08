namespace HiTechStore.Data.DTOs.Category
{
    public class CategoryUpdateDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int? ParentCategoryId { get; set; }
        public IFormFile? Image { get; set; }
        public IFormFile? Icon { get; set; }

    }
}