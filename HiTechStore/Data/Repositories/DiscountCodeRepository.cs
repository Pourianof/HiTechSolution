using AutoMapper;

using HiTechStore.Core.Repositories;
using HiTechStore.Models;

namespace HiTechStore.Data.Repositories;

public class DiscountCodeRepository : Repository<DiscountCode>, IDiscountCodeRepository
{
    public DiscountCodeRepository(HiTechStoreDbContext context, IMapper mapper) : base(context, mapper)
    {
    }


}
