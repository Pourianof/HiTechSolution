using HiTechStore.Core;
using HiTechStore.Core.Repositories;
using HiTechStore.Data.Repositories;

namespace HiTechStore.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly HiTechStoreDbContext _context;
        public IProductRepository Products { get; }
        public ICategoryRepository Categories { get; }

        public UnitOfWork(HiTechStoreDbContext context)
        {
            _context = context;
            Products = new ProductRepository(_context);
            Categories = new CategoryRepository(_context);
        }

        public async Task<int> Complete()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
