using HiTechStore.Core.Common.Interfaces.Infra.Repositories;

namespace HiTechStore.Infrastructure.Data.Repositories;

public static class RepositoryDependenciesRegistration
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddTransient<IProductRepository, ProductRepository>();
        services.AddTransient<IBrandRepository, BrandRepository>();
        services.AddTransient<IBrandModelRepository, BrandModelRepository>();
        services.AddTransient<ICartRepository, CartRepository>();
        services.AddTransient<ICategoryRepository, CategoryRepository>();
        services.AddTransient<IColorRepository, ColorRepository>();
        services.AddTransient<IComponentRepository, ComponentRepository>();
        services.AddTransient<IConditionMethodRepository, ConditionMethodRepository>();
        services.AddTransient<IDiscountCodeRepository, DiscountCodeRepository>();
        services.AddTransient<IDiscountEntityRepository, DiscountEntityRepository>();
        services.AddTransient<IFilterRepository, FilterRepository>();
        services.AddTransient<IOrderRepository, OrderRepository>();
        services.AddTransient<IProductScoresRepository, ProductScoresRepository>();
        services.AddTransient<IUserRepository, UserRepository>();
        services.AddTransient<IDiscountedProductsRepository, DiscountedProductsRepository>();
        services.AddTransient<ICommentRepository, CommentRepository>();
        services.AddTransient<IProductVariationRepository, ProductVariationRepository>();

        return services;
    }
}