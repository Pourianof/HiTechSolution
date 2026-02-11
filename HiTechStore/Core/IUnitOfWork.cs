using HiTechStore.Core.Repositories;

namespace HiTechStore.Core
{
    public interface IUnitOfWork : IDisposable
    {
        IProductRepository Products { get; }
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
        IDiscountCodeRepository DiscountCodeRepository { get; }

        IRepositoryModelBase<TModel> RespositoryOf<TModel>() where TModel : class, IModel;
        Task<int> Complete();
    }
}
