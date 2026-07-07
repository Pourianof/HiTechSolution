using HiTechStore.Core.Common.Interfaces.Infra;
using HiTechStore.Core.Common.Interfaces.Infra.Repositories;

namespace HiTechStore.Core
{
    public interface IUnitOfWork : IDisposable
    {
        IProductRepository Products { get; }
        IProductVariationRepository ProductVariationRepository { get; }
        ICategoryRepository Categories { get; }
        IProductScoresRepository ProductScores { get; }
        IComponentRepository ComponentRepository { get; }
        IBrandRepository BrandRepository { get; }
        IBrandModelRepository BrandModelRepository { get; }
        IFilterRepository FilterRepository { get; }
        ICartRepository CartRepository { get; }
        IOrderRepository OrderRepository { get; }
        IColorRepository ColorRepository { get; }
        IDiscountEntityRepository DiscountEntityRepository { get; }
        IDiscountCodeRepository DiscountRepository { get; }
        IConditionMethodRepository ConditionMethodRepository { get; }
        IUserRepository UserRepository { get; }
        IDiscountedProductsRepository DiscountedProductsRepository { get; }
        ICommentRepository CommentRepository { get; }
        IPermissionRepository PermissionRepository { get; }
        IPermissionAuditRepository PermissionAuditRepository { get; }
        IRepositoryModelBase<TModel, TId> RespositoryOf<TModel, TId>()
            where TModel : class, IModel
            where TId : struct;
        Task<int> Complete();
        Task<ITransaction> StartTransaction();
    }
}


public interface ITransaction : IDisposable, IAsyncDisposable
{
    Task Commit();
    Task Rollback();
}