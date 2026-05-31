namespace HiTechStore.Core.Services.ProductScore;


public interface IProductScoreService
{
    Task<Core.Models.ProductScore> AddScoreForProduct(
        ProdcutScoreCreationServiceDto prodcutScoreCreationDto
    );
}


public class ProdcutScoreCreationServiceDto
{
    public int ProductId { get; set; }
    public int Score { get; set; }
}