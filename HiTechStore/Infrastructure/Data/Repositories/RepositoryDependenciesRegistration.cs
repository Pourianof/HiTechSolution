using HiTechStore.Core.Common.Interfaces.Infra;
using HiTechStore.Core.Common.Interfaces.Infra.Repositories;

namespace HiTechStore.Infrastructure.Data.Repositories;

public static class RepositoryDependenciesRegistration
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IBrandRepository, BrandRepository>();
        services.AddScoped<IBrandModelRepository, BrandModelRepository>();
        services.AddScoped<ICartRepository, CartRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IColorRepository, ColorRepository>();
        services.AddScoped<IComponentRepository, ComponentRepository>();
        services.AddScoped<IConditionMethodRepository, ConditionMethodRepository>();
        services.AddScoped<IDiscountCodeRepository, DiscountCodeRepository>();
        services.AddScoped<IDiscountEntityRepository, DiscountEntityRepository>();
        services.AddScoped<IFilterRepository, FilterRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IProductScoresRepository, ProductScoresRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IDiscountedProductsRepository, DiscountedProductsRepository>();
        services.AddScoped<ICommentRepository, CommentRepository>();
        services.AddScoped<IProductVariationRepository, ProductVariationRepository>();
        services.AddScoped<IPermissionRepository, PermissionRepository>();
        services.AddScoped<IPermissionAuditRepository, PermissionAuditRepository>();
        services.AddTransient<IUserNotificationRepository, UserNotificationRepository>();
        services.AddScoped<OutboxMessageRepository>();

        return services;
    }
}