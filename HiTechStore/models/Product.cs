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
        public string? Image { get; set; }
        public string? AuthorId { get; set; }
        public virtual User? Author { get; set; }
        public virtual List<ProductCategory>? Categories { get; set; }
        public virtual ICollection<ProductScore> Scores { get; set; } = new List<ProductScore>();
        [NotMapped]
        public double? AverageScore { get; set; } = 0.0;
        [NotMapped]
        public int ScoreCounts { get; set; } = 0;
        [NotMapped]
        public int? MyScore { get; set; }
    }

    public class ProductCategory
    {
        public int ProductId { get; set; }
        public int CategoryId { get; set; }
        public virtual Category? category { get; set; }
    }
}