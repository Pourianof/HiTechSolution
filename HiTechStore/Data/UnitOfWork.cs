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
        public IDiscountCodeRepository DiscountRepository { get; }
        public IDiscountEntityRepository DiscountEntityRepository { get; }
        public IConditionMethodRepository ConditionMethodRepository { get; }

        public UnitOfWork(
            HiTechStoreDbContext context,
            IMapper mapper,
            IProductRepository productRepository,
            ICategoryRepository categoryRepository,
            IProductScoresRepository productScoresRepository,
            IComponentRepository componentRepository,
            IBrandRepository brandRepository,
            IBrandModelRepository brandModelRepository,
            IFilterRepository filterRepository,
            IColorRepository colorRepository,
            ICartRepository cartRepository,
            IOrderRepository orderRepository,
            IDiscountCodeRepository discountCodeRepository,
            IDiscountEntityRepository discountEntityRepository,
            IConditionMethodRepository conditionMethodRepository
        )
        {
            _context = context;
            _mapper = mapper;
            Products = productRepository;
            Categories = categoryRepository;
            ProductScores = productScoresRepository;
            ComponentRepository = componentRepository;
            BrandRepository = brandRepository;
            BrandModelRepository = brandModelRepository;
            FilterRepository = filterRepository;
            CartRepository = cartRepository;
            OrderRepository = orderRepository;
            ColorRepository = colorRepository;
            DiscountRepository = discountCodeRepository;
            DiscountEntityRepository = discountEntityRepository;
            ConditionMethodRepository = conditionMethodRepository;
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
