using HiTechStore.Data.DTOs.DiscountEntity;
using HiTechStore.Models;

namespace HiTechStore.Core.Repositories;

public interface IDiscountEntityRepository : IRepository<DiscountEntity, DiscountEntityDto>
{
    Task<DiscountEntityProperty?> GetPropertyById(int propertyId);
    Task<DiscountEntityProperty?> GetPropertyByPathAsync(string path);
    Task<ConditionMethod?> GetConditionMethodByNameAsync(string methodName);
    Task AddAllSafeAsync(IEnumerable<DiscountEntity> discountEntities);
}
