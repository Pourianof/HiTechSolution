using HiTechStore.Core.Helpers;
using HiTechStore.Core.Services.Authorization;
using HiTechStore.Core.Services.Discount;
using HiTechStore.Core.Services.Product;

namespace HiTechStore.Core.Services;

public static class ServicesDependencyRegistration
{
    static public IServiceCollection AddAppServices(this IServiceCollection services)
    {
        services.AddScoped<IDiscountCodeGenerator, DiscountCodeGenerator>();
        services.AddScoped<IDiscountService, DiscountService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IAuthorizationService, AuthorizationService>();

        return services;
    }
}
