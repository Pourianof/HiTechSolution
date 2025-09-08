using AutoMapper;

using HiTechStore.Models;
using HiTechStore.DTOs.Product;
using HiTechStore.Data.DTOs.Authorization;
using HiTechStore.Data.DTOs.Category;
using HiTechStore.Data.DTOs;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Product, ProductDTO>();
        CreateMap<ProductDTO, Product>().ForMember(dest => dest.Categories, opt => opt.Ignore());
        CreateMap<ProductPatchDTO, Product>().MapOnlyNonNull();
        CreateMap<Product, ProductPatchDTO>();
        CreateMap<CategoryUpdateDto, Category>().MapOnlyNonNull();
        CreateMap<Category, CategoryDTO>();
        CreateMap<RegisterDto, User>();
        CreateMap<User, RegisterDto>();
    }
}

static class MappingExtensions
{
    public static IMappingExpression<TSource, TDestination> MapOnlyNonNull<TSource, TDestination>(
        this IMappingExpression<TSource, TDestination> mappingExpression)
    {
        mappingExpression.ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        return mappingExpression;
    }
}