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

    }
}