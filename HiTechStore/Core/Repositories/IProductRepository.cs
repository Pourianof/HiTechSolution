using HiTechStore.Models;

namespace HiTechStore.Core.Repositories
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<Product?> GetByIdAsync(int id, string? userId);
    }
}
