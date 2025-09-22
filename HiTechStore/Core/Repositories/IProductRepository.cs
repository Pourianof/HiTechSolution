using HiTechStore.Data.Queries;
using HiTechStore.Models;

namespace HiTechStore.Core.Repositories
{
    public interface IProductRepository : IRepository<Product, ProductQuery>
    {
        Task<Product?> GetByIdAsync(int id, string? userId);
    }
}
