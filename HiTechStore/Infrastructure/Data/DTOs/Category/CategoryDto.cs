using HiTechStore.Infrastructure.Data.DTOs.Component;

namespace HiTechStore.Infrastructure.Data.DTOs
{
    public class CategoryDTO
    {
        public int CategoryId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int? ParentCategoryId { get; set; }
        public string? Image { get; set; }
        public string? Icon { get; set; }
        public IEnumerable<PropertyDto>? Properties { get; set; }
        public IEnumerable<ComponentTypeWithPropertiesDto>? Components { get; set; }

    }
}