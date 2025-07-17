using HiTechStore.Core;
using HiTechStore.Core.Repositories;
using HiTechStore.Data.Repositories;

namespace HiTechStore.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly HiTechStoreDbContext _context;
        private readonly IProductRepository _productRepository;

        public UnitOfWork(HiTechStoreDbContext context)
        {
            _context = context;
            _productRepository = new ProductRepository(_context);
        }

        public IProductRepository Products => _productRepository;

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
