using HiTechStore.Core;

namespace HiTechStore.Core.Models
{
    public class ProductScore : IModel
    {
        public int ProductScoreId { get; set; }
        public int ProductId { get; set; }
        public int Score { get; set; }
        public string? UserId { get; set; }
    }
}