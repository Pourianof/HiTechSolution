using HiTechStore.Models;

namespace HiTechStore.Core.Repositories
{
    public interface IProductScoresRepository : IRepository<ProductScore>
    {
        Task<IEnumerable<ProductScore>> GetUserScoresAsync(string userId);
        Task<ProductScore?> GetUserScoreForProductAsync(string userId, int productId);
    }
}
