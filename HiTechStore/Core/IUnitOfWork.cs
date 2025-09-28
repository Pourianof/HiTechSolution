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
        Task<int> Complete();
    }
}
