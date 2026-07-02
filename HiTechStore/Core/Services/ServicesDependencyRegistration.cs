using HiTechStore.Core.Helpers;
using HiTechStore.Core.Services.Authorization;
using HiTechStore.Core.Services.Cart;
using HiTechStore.Core.Services.Comment;
using HiTechStore.Core.Services.Discount;
using HiTechStore.Core.Services.Permission;
using HiTechStore.Core.Services.Product;
using HiTechStore.Core.Services.ProductComment;
using HiTechStore.Core.Services.ProductScore;
using HiTechStore.Core.Services.ProductVariation;
using HiTechStore.Core.Services.UserService;

namespace HiTechStore.Core.Services;

public static class ServicesDependencyRegistration
{
    static public IServiceCollection AddAppServices(this IServiceCollection services)
    {
        services.AddScoped<IDiscountCodeGenerator, DiscountCodeGenerator>();
        services.AddScoped<IDiscountService, DiscountService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IAuthorizationService, AuthorizationService>();
        services.AddScoped<ICommentService, CommentService>();
        services.AddScoped<IProductCommentService, ProductCommentService>();
        services.AddScoped<IProductScoreService, ProductScoreService>();
        services.AddScoped<IProductVariationService, ProductVariationService>();
        services.AddScoped<IUserService, UserService.UserService>();
        services.AddScoped<ICartService, CartService>();
        services.AddScoped<IPermissionService, PermissionService>();

        services.AddScoped<ProductPermissionHelper>();
        services.AddScoped<DiscountPermissionHelper>();

        services.AddTransient<ProductServiceHelper>();

        return services;
    }
}
