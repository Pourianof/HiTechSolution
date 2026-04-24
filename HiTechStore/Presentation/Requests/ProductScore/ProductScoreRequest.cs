namespace HiTechStore.Presentation.Requests.ProductScore;


using System.ComponentModel.DataAnnotations;


public class ProductScoreRequest
{
    [Required]
    [Range(1, 5)]
    public int Score { get; set; }
}