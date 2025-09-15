using System.ComponentModel.DataAnnotations;

using HiTechStore.Data.DTOs.Product;
using HiTechStore.Data.DTOs.Product.Validations;

namespace HiTechStore.DTOs.Product
{
    public class ProductCreationDto
    {
        [Required]
        [MinLength(3)]
        [MaxLength(100)]
        public string? Title { get; set; }

        [Required]
        [Range(0, 10000000)]
        public decimal? Price { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }
        [ProductMediaValidation]
        public IEnumerable<IFormFile>? Media { get; set; }
        public virtual IEnumerable<int>? Categories { get; set; }
        public MediaMetaDataDto? MediaMetaData;
    }
}

