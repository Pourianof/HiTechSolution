using HiTechStore.Infrastructure.Data.DTOs.DiscountEntity;
using HiTechStore.Core.Models;

namespace HiTechStore.Core.Common.Interfaces.Infra.Repositories;

public interface IDiscountEntityRepository : IRepositoryWithIntegerId<DiscountEntity, DiscountEntityDto>
{
    Task<DiscountEntityProperty?> GetPropertyById(int propertyId);
    Task<DiscountEntityProperty?> GetPropertyByEntityAsync(string entityName, string propertyName);
    Task<ConditionMethod?> GetConditionMethodByNameAsync(string methodName);
    Task AddAllSafeAsync(IEnumerable<DiscountEntity> discountEntities);
}
