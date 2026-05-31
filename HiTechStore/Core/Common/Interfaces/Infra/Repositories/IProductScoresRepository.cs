using HiTechStore.Core.Models;

namespace HiTechStore.Core.Common.Interfaces.Infra.Repositories
{
    public interface IProductScoresRepository : IRepository<ProductScore>
    {
        Task<IEnumerable<ProductScore>> GetUserScoresAsync(string userId);
        Task<ProductScore?> GetUserScoreForProductAsync(string userId, int productId);
    }
}
