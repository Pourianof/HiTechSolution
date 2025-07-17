using AutoMapper;

using HiTechStore.Models;
using HiTechStore.DTOs.Product;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Product, ProductDTO>();
        CreateMap<ProductDTO, Product>();
        CreateMap<ProductPatchDTO, Product>().ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}