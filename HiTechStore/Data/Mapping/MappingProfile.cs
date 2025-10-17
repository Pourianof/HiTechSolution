using AutoMapper;

using HiTechStore.Models;
using HiTechStore.DTOs.Product;
using HiTechStore.Data.DTOs.Authorization;
using HiTechStore.Data.DTOs.Category;
using HiTechStore.Data.DTOs;
using HiTechStore.Data.DTOs.Product;
using HiTechStore.Data.DTOs.Component;
using HiTechStore.DTOs.Category;
using HiTechStore.Data.DTOs.Brand;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        ProductMap();
        CategoryMap();
        PropertyMap();
        UserMap();
        ComponentMap();
        BrandMap();
    }

    private void ProductMap()
    {
        CreateMap<Product, ProductCreationDto>();
        CreateMap<ProductCreationDto, Product>()
            .ForMember(dest => dest.Category, opt => opt.Ignore())
            .ForMember(dest => dest.Media, opt => opt.Ignore())
            .ForMember(dest => dest.CategoryId, opt => opt.Ignore())
            .ForMember(dest => dest.BrandModel, opt => opt.Ignore());
        CreateMap<ProductPatchDTO, Product>().MapOnlyNonNull();
        CreateMap<Product, ProductPatchDTO>();
        CreateMap<ProductPropertyValue, PropertyValueDto>()
            .ForMember((dest) => dest.Name, (opt) => opt.MapFrom((p) => p.Property!.Name))
            .ForMember(dest => dest.ValueType, opt => opt.MapFrom(src => src.Property!.PropertyType));
        CreateMap<ProductMedia, ProductMediaDto>().ForMember((dest) => dest.Url, (opt) => opt.MapFrom(src => src.FilePath));
    }

    private void CategoryMap()
    {
        CreateMap<CategoryUpdateDto, Category>().MapOnlyNonNull();
        CreateMap<CategoryCreationDto, Category>();
        CreateMap<Category, CategoryDTO>()
            .ForMember((dest) => dest.Properties, (opt) => opt.MapFrom(src => src.Properties))
            .ForMember((dest) => dest.Components, (opt) => opt.MapFrom(src => src.Components!.Select(cmp => cmp.Component)));
    }

    private void ComponentMap()
    {
        CreateMap<ComponentModel, ComponentModelDto>();
        CreateMap<ComponentModelCreationDto, ComponentModel>();
        CreateMap<ComponentType, ComponentTypeDto>();
        CreateMap<ComponentType, ComponentTypeWithPropertiesDto>()
            .ForMember((dest) => dest.ComponentTypeId, opt => opt.MapFrom(src => src.ComponentTypeId));
        CreateMap<ComponentCreationOrReferenceDto, ComponentType>();
        CreateMap<ComponentCreationDto, ComponentType>();
        CreateMap<ComponentCreationOrReferenceDto, CategoryComponent>()
            .ForMember(
                dest => dest.Component,
                opt =>
                {
                    opt.Condition((src) => src.ComponentTypeId == null);
                    opt.MapFrom(src => src.NewComponent);
                }
            );
    }

    private void PropertyMap()
    {
        CreateMap<Property, PropertyDto>();
        CreateMap<PropertyEntryCreationDto, Property>();
        CreateMap<PropertyValueEntryCreationDto, ComponentPropertyValue>()
            .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src));
        CreateMap<PropertyValueEntryCreationDto, PropertyValue>()
            .ForMember(dest => dest.ValueString, opt => opt.MapFrom(src => src.PropertyValue));
        CreateMap<ComponentPropertyValue, PropertyValueDto>()
            .ForMember((dest) => dest.Name, opt => opt.MapFrom(src => src.Property == null ? null : src.Property.Name))
            .ForMember((dest) => dest.Value, opt => opt.MapFrom(src => src.Value != null ?
                    src.Value.ValueNumber != null ? (object?)src.Value.ValueNumber :
                    src.Value.ValueDateTime != null ? (object?)src.Value.ValueDateTime :
                    src.Value.ValueBoolean != null ? (object?)src.Value.ValueBoolean :
                    src.Value.ValueString
                : null));
    }

    private void UserMap()
    {
        CreateMap<RegisterDto, User>();
        CreateMap<User, RegisterDto>();
    }

    private void BrandMap()
    {
        CreateMap<BrandModel, BrandModelDto>()
            .ForMember(dest => dest.BrandName, opt => opt.MapFrom((src) => src.Brand!.Name))
            .ForMember(dest => dest.ModelName, opt => opt.MapFrom((src) => src.Name))
            .ForMember(dest => dest.ModelId, opt => opt.MapFrom((src) => src.BrandModelId));

        CreateMap<Brand, BrandDto>();
        CreateMap<BrandCreationDto, Brand>();
        CreateMap<BrandModelCreationDto, BrandModel>();

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