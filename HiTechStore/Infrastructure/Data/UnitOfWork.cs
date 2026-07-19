using AutoMapper;

using HiTechStore.Core;
using HiTechStore.Core.Common.Interfaces.Infra;
using HiTechStore.Core.Common.Interfaces.Infra.Repositories;
using HiTechStore.Infrastructure.Data.Repositories;
using HiTechStore.Infrastructure.Helpers;

using Microsoft.EntityFrameworkCore.Storage;

namespace HiTechStore.Infrastructure.Data
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
        public ICartRepository CartRepository { get; }
        public IOrderRepository OrderRepository { get; }
        public IColorRepository ColorRepository { get; }
        public IDiscountCodeRepository DiscountRepository { get; }
        public IDiscountEntityRepository DiscountEntityRepository { get; }
        public IConditionMethodRepository ConditionMethodRepository { get; }
        public IUserRepository UserRepository { get; }
        public IDiscountedProductsRepository DiscountedProductsRepository { get; }
        public ICommentRepository CommentRepository { get; }
        public IProductVariationRepository ProductVariationRepository { get; }
        public IPermissionRepository PermissionRepository { get; }
        public IPermissionAuditRepository PermissionAuditRepository { get; }
        public IUserNotificationRepository UserNotificationRepository { get; }
        private OutboxMessageRepository _outboxMessageRepository;
        private OutboxSignal _outboxSignal;

        public UnitOfWork(
            HiTechStoreDbContext context,
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
            IConditionMethodRepository conditionMethodRepository,
            IUserRepository userRepository,
            IDiscountedProductsRepository discountedProductsRepository,
            ICommentRepository commentRepository,
            IProductVariationRepository productVariationRepository,
            IPermissionRepository permissionRepository,
            IPermissionAuditRepository permissionAuditRepository,
            IUserNotificationRepository userNotificationRepository,
            OutboxMessageRepository outboxMessageRepository,
            OutboxSignal outboxSignal
        )
        {
            _context = context;

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
            UserRepository = userRepository;
            DiscountedProductsRepository = discountedProductsRepository;
            CommentRepository = commentRepository;
            ProductVariationRepository = productVariationRepository;
            PermissionRepository = permissionRepository;
            PermissionAuditRepository = permissionAuditRepository;
            UserNotificationRepository = userNotificationRepository;

            _outboxMessageRepository = outboxMessageRepository;
            _outboxSignal = outboxSignal;
        }

        public HiTechStoreDbContext Context()
        {
            return _context;
        }

        public async Task<int> Complete()
        {
            var result = await _context.SaveChangesAsync();

            Console.WriteLine($"HAS PENDING: {_outboxMessageRepository.HasPendingMessages}");

            if (result > 0 && _outboxMessageRepository.HasPendingMessages)
            {
                _outboxSignal.Notify();
                _outboxMessageRepository.Reset();
            }

            return result;
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        IRepositoryModelBase<TModel, TId> IUnitOfWork.RespositoryOf<TModel, TId>()
        {
            return new RepositoryCore<TModel, TId>(_context);
        }

        public async Task<ITransaction> StartTransaction()
        {
            var trx = await _context.Database.BeginTransactionAsync();

            return new Transaction(trx);
        }
    }
}


class Transaction(IDbContextTransaction dbTrx) : ITransaction
{
    public Task Commit()
    {
        return dbTrx.CommitAsync();
    }

    public Task Rollback()
    {
        return dbTrx.RollbackAsync();
    }

    public void Dispose()
    {
        dbTrx.Dispose();
    }

    public ValueTask DisposeAsync()
    {
        return dbTrx.DisposeAsync();
    }
}