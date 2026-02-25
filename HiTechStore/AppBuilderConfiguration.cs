using System.Reflection;

using HiTechStore.Controllers.ExceptionFilters;
using HiTechStore.Core.Auth;
using HiTechStore.Core.ExceptionHandlers;
using HiTechStore.Core.Services;
using HiTechStore.Data;
using HiTechStore.Helpers.AutoMapper;
using HiTechStore.Helpers.URLFilterQuery;

namespace HiTechStore;

public static class DependencyRegistration
{
    public static WebApplicationBuilder ConfigueBuilder(this WebApplicationBuilder builder)
    {
        builder.Services.AddControllers(
            (options) =>
            {
                options.ModelBinderProviders.Insert(0, new ToQueryModelBinderProvider());
            }
        );

        builder.UseDataAccess();

        builder.Services.AddAutoMapper((cfg) =>
        {
            cfg.RegisterAttributeMaps(Assembly.GetExecutingAssembly());
            cfg.AddProfile(typeof(MappingProfile));
        });
        builder.Services.AddQueryParser();
        builder.Services.AddAppServices();

        builder.AddAuth();

        builder.Services.AddExceptionHandler<ApplicationExceptionHandler>();
        builder.Services.AddExceptionHandler<PgDbExceptionHandler>();
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

        builder.Services.UseHiTechPaySdk();

        return builder;
    }
}