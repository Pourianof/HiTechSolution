using AutoMapper;

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
        public IProductScoresRepository ProductScores { get; }
        public IComponentRepository ComponentRepository { get; }
        public IBrandRepository BrandRepository { get; }
        public IBrandModelRepository BrandModelRepository { get; }
        public IFilterRepository FilterRepository { get; }


        public UnitOfWork(HiTechStoreDbContext context, IMapper mapper)
        {
            _context = context;
            Products = new ProductRepository(_context, mapper);
            Categories = new CategoryRepository(_context, mapper);
            ProductScores = new ProductScoresRepository(_context, mapper);
            ComponentRepository = new ComponentRepository(_context, mapper);
            BrandRepository = new BrandRepository(_context, mapper);
            BrandModelRepository = new BrandModelRepository(_context, mapper);
            FilterRepository = new FilterRepository(_context);
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
