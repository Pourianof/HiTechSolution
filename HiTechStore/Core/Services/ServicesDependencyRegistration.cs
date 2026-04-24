using HiTechStore.Core.Helpers;
using HiTechStore.Core.Services.Authorization;
using HiTechStore.Core.Services.Comment;
using HiTechStore.Core.Services.Discount;
using HiTechStore.Core.Services.Product;
using HiTechStore.Core.Services.ProductComment;
using HiTechStore.Core.Services.ProductScore;

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

        return services;
    }
}
