using HiTechStore.Data.DTOs.DiscountEntity;
using HiTechStore.Models;

namespace HiTechStore.Core.Repositories;

public interface IDiscountEntityRepository : IRepository<DiscountEntity, DiscountEntityDto>
{
    Task<DiscountEntityProperty?> GetPropertyById(int propertyId);
}
