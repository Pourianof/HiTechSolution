using AutoMapper;

using HiTechStore.Core.Repositories;
using HiTechStore.Data.DTOs.DiscountEntity;
using HiTechStore.Models;

namespace HiTechStore.Data.Repositories;

public class DiscountEntityRepository : Repository<DiscountEntity, DiscountEntityDto>, IDiscountEntityRepository
{
    public DiscountEntityRepository(HiTechStoreDbContext context, IMapper mapper) : base(context, mapper)
    {
    }


}
