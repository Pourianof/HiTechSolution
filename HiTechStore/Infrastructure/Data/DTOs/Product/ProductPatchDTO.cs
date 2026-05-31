using System.ComponentModel.DataAnnotations;

namespace HiTechStore.DTOs.Product
{
    public class ProductPatchDTO
    {
        [MinLength(3)]
        [MaxLength(100)]
        public string? Title { get; set; }

        [Range(0, 10000000)]
        public decimal? Price { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }
    }
}