using System.ComponentModel.DataAnnotations.Schema;

using HiTechStore.Core;

namespace HiTechStore.Models
{
    public class Product : IModel
    {
        public int ProductId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? AuthorId { get; set; }
        public virtual int BrandModelId { get; set; }
        public virtual BrandModel? BrandModel { get; set; }
        public virtual User? Author { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool? IsDeleled { get; set; } = false;
        public virtual int CategoryId { get; set; }
        public virtual Category? Category { get; set; }
        public virtual ICollection<Comment>? Comments { get; set; } = new List<Comment>();
        public virtual ICollection<ProductScore> Scores { get; set; } = new List<ProductScore>();
        public double? AverageScore { get; set; } = 0.0;
        public int ScoreCounts { get; set; } = 0;
        [NotMapped]
        public int? MyScore { get; set; }
        public virtual List<ProductPropertyValue> Properties { get; set; } = new();
        public virtual List<ComponentModel> ComponentModels { get; set; } = new();
        public virtual List<ProductVariation> Variations { get; set; } = new();
    }

    public class ProductPropertyValue : BaseItemPropertyValue
    {
        public int ProductId { get; set; }
        public virtual Product? Product { get; set; }
        [ForeignKey("ProductPropertyValue")]
        public override PropertyValue? Value { get; set; }
    }

    [Table("ProductVariation")] // just for temperory alignment with DbSet<ProductVariation> ProductVariations in DbContext
    public class ProductVariation : IModel
    {
        public int ProductVariationId { get; set; }
        public double Price { get; set; }
        public int ColorId { get; set; }
        public virtual Color? Color { get; set; }
        public int Inventory { get; set; }
        public virtual List<ProductMedia> Media { get; set; } = new();
        public int ProductId { get; set; }
        public virtual Product? Product { get; set; }
        public virtual IEnumerable<OrderItem>? Orders { get; set; }
    }
}