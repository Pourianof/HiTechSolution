using System.ComponentModel.DataAnnotations;

namespace HiTechStore.Data.DTOs.Product
{
    public class ProductScoreDto
    {
        [Required]
        [Range(1, 5)]
        public int Score { get; set; }
    }
}