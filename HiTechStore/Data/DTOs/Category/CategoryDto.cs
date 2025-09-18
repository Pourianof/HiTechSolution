namespace HiTechStore.Data.DTOs
{
    public class CategoryDTO
    {
        public int CategoryId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int? ParentCategoryId { get; set; }
        public string? Image { get; set; }
        public IEnumerable<CategoryPropertyDto>? Properties { get; set; }
    }

    public class CategoryPropertyDto
    {
        public int PropertyId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
    }
}