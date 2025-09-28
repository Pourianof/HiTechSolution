using AutoMapper;

using HiTechStore.Core.Repositories;
using HiTechStore.Data.DTOs.Brand;
using HiTechStore.Models;

namespace HiTechStore.Data.Repositories;

public class BrandRepository : Repository<Brand, BrandDto>, IBrandRepository
{
    public BrandRepository(HiTechStoreDbContext context, IMapper mapper) : base(context, mapper) { }


}