using System.Reflection;

using HiTechStore.Controllers.ExceptionFilters;
using HiTechStore.Core.Auth;
using HiTechStore.Core.BackgroundJobs;
using HiTechStore.Core.ExceptionHandlers;
using HiTechStore.Core.Services;
using HiTechStore.Data;
using HiTechStore.Data.Mapping;
using HiTechStore.Helpers.URLFilterQuery;
using HiTechStore.Presentation;

namespace HiTechStore;

public static class DependencyRegistration
{
    public static WebApplicationBuilder ConfigueBuilder(this WebApplicationBuilder builder)
    {
        builder.Services.AddLogging();

        builder.Services.AddBackroundJobs();

        builder.Services.AddControllers(
            (options) =>
            {
                options.ModelBinderProviders.Insert(0, new ToQueryModelBinderProvider());
            }
        );

        builder.UseDataAccess();

        builder.Services.AddMapping();

        builder.Services.AddQueryParser();
        builder.Services.AddAppServices();

        builder.AddAuth();

        builder.Services.AddPresentation();

        builder.Services.AddExceptionHandler<ApplicationExceptionHandler>();
        builder.Services.AddExceptionHandler<PgDbExceptionHandler>();
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

        builder.Services.UseHiTechPaySdk();

        return builder;
    }
}