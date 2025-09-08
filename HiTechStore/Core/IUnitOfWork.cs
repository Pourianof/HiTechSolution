using HiTechStore.Core.Repositories;

namespace HiTechStore.Core
{
    public interface IUnitOfWork : IDisposable
    {
        IProductRepository Products { get; }
        ICategoryRepository Categories { get; }
        Task<int> Complete();
    }
}
