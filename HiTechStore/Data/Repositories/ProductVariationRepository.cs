using AutoMapper;

using HiTechStore.Core.Repositories;
using HiTechStore.Data.DTOs;
using HiTechStore.Models;

namespace HiTechStore.Data.Repositories;

public class ProductVariationRepository : Repository<ProductVariation, ProductVariationDto>, IProductVariationRepository
{
    public ProductVariationRepository(HiTechStoreDbContext context, IMapper mapper) : base(context, mapper)
    {
    }
}