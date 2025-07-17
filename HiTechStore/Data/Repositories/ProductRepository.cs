using HiTechStore.Core.Repositories;
using HiTechStore.Models;

namespace HiTechStore.Data.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(HiTechStoreDbContext context) : base(context)
        {
        }
    }
}
