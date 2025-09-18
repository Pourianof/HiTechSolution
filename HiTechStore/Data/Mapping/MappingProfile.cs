using AutoMapper;

using HiTechStore.Models;
using HiTechStore.DTOs.Product;
using HiTechStore.Data.DTOs.Authorization;
using HiTechStore.Data.DTOs.Category;
using HiTechStore.Data.DTOs;
using HiTechStore.Data.DTOs.Product;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Product, ProductCreationDto>();
        CreateMap<ProductCreationDto, Product>()
            .ForMember(dest => dest.Category, opt => opt.Ignore())
            .ForMember(dest => dest.Media, opt => opt.Ignore())
            .ForMember(dest => dest.CategoryId, opt => opt.Ignore());
        CreateMap<ProductPatchDTO, Product>().MapOnlyNonNull();
        CreateMap<Product, ProductPatchDTO>();
        CreateMap<Product, ProductDto>();
        CreateMap<ProductMedia, ProductMediaDto>().ForMember((dest) => dest.Url, (opt) => opt.MapFrom(src => src.FilePath));
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