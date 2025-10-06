using System.ComponentModel.DataAnnotations.Schema;

using HiTechStore.Core;

namespace HiTechStore.Models
{
    public class Product : IModel
    {
        public int ProductId { get; set; }
        public double Price { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? AuthorId { get; set; }
        public virtual BrandModel? BrandModel { get; set; }
        public virtual User? Author { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool? IsDeleled { get; set; } = false;
        public virtual List<ProductMedia> Media { get; set; } = new();
        public virtual int CategoryId { get; set; }
        public virtual Category? Category { get; set; }
        public virtual ICollection<ProductScore> Scores { get; set; } = new List<ProductScore>();
        [NotMapped]
        public double? AverageScore { get; set; } = 0.0;
        [NotMapped]
        public int ScoreCounts { get; set; } = 0;
        [NotMapped]
        public int? MyScore { get; set; }
        public virtual List<ProductPropertyValue> Properties { get; set; } = new();
        public virtual List<ComponentModel> ComponentModels { get; set; } = new();
    }

    public class ProductPropertyValue : BaseItemPropertyValue
    {
        public int ProductId { get; set; }
        public virtual Product? Product { get; set; }
        [ForeignKey("ProductPropertyValue")]
        public override PropertyValue? Value { get; set; }
    }
}