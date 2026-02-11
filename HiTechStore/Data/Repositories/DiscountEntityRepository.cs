using AutoMapper;

using HiTechStore.Core.Repositories;
using HiTechStore.Models;

namespace HiTechStore.Data.Repositories;

public class DiscountEntityRepository : Repository<DiscountEntity>, IDiscountEntityRepository
{
    public DiscountEntityRepository(HiTechStoreDbContext context, IMapper mapper) : base(context, mapper)
    {
    }


}
