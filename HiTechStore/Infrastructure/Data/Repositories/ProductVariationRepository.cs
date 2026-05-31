using AutoMapper;

using HiTechStore.Core.Common.Interfaces.Infra.Repositories;
using HiTechStore.Infrastructure.Data.DTOs;
using HiTechStore.Core.Models;

namespace HiTechStore.Infrastructure.Data.Repositories;

public class ProductVariationRepository : Repository<ProductVariation, ProductVariationDto>, IProductVariationRepository
{
    public ProductVariationRepository(HiTechStoreDbContext context, IMapper mapper) : base(context, mapper)
    {
    }
}