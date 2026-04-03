using AutoMapper;

using HiTechStore.Core;
using HiTechStore.Core.Repositories;
using HiTechStore.Data.Repositories;

namespace HiTechStore.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly HiTechStoreDbContext _context;
        private readonly IMapper _mapper;
        public IProductRepository Products { get; }
        public ICategoryRepository Categories { get; }
        public IProductScoresRepository ProductScores { get; }
        public IComponentRepository ComponentRepository { get; }
        public IBrandRepository BrandRepository { get; }
        public IBrandModelRepository BrandModelRepository { get; }
        public IFilterRepository FilterRepository { get; }
        public ICartRepository CartRepository { get; }
        public IOrderRepository OrderRepository { get; }
        public IColorRepository ColorRepository { get; }
        public IDiscountCodeRepository DiscountCodeRepository { get; }
        public IDiscountEntityRepository DiscountEntityRepository { get; }
        public IConditionMethodRepository ConditionMethodRepository { get; }

        public UnitOfWork(HiTechStoreDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            Products = new ProductRepository(_context, mapper);
            Categories = new CategoryRepository(_context, mapper);
            ProductScores = new ProductScoresRepository(_context, mapper);
            ComponentRepository = new ComponentRepository(_context, mapper);
            BrandRepository = new BrandRepository(_context, mapper);
            BrandModelRepository = new BrandModelRepository(_context, mapper);
            FilterRepository = new FilterRepository(_context);
            CartRepository = new CartRepository(_context, mapper);
            OrderRepository = new OrderRepository(_context, mapper);
            ColorRepository = new ColorRepository(_context, mapper);
            DiscountCodeRepository = new DiscountCodeRepository(_context, mapper);
            DiscountEntityRepository = new DiscountEntityRepository(_context, mapper);
            ConditionMethodRepository = new ConditionMethodRepository(_context, mapper);
        }

        public HiTechStoreDbContext Context()
        {
            return _context;
        }

        public async Task<int> Complete()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        IRepositoryModelBase<TModel> IUnitOfWork.RespositoryOf<TModel>()
        {
            return new RepositoryCore<TModel>(_context);
        }
    }
}
