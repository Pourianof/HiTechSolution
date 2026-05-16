using HiTechStore.Core.Repositories;

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
        IRepositoryModelBase<TModel> RespositoryOf<TModel>() where TModel : class, IModel;
        Task<int> Complete();
        Task<ITransaction> StartTransaction();
    }
}


public interface ITransaction : IDisposable, IAsyncDisposable
{
    Task Commit();
    Task Rollback();
}