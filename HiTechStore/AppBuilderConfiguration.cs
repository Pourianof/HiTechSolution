using HiTechStore.Core.BackgroundJobs;
using HiTechStore.Core.Services;
using HiTechStore.Infrastructure.Data;
using HiTechStore.Infrastructure.Data.Mapping;
using HiTechStore.Helpers.URLFilterQuery;
using HiTechStore.Infrastructure;
using HiTechStore.Presentation;
using HiTechStore.Core.Common.Interfaces.Presentation;

namespace HiTechStore;

public static class DependencyRegistration
{
    public static WebApplicationBuilder ConfigueBuilder(this WebApplicationBuilder builder)
    {
        builder.Services.AddSignalR();

        builder.Services.AddHealthChecks();

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

        builder.UsePresentation();

        builder.Services.AddProblemDetails();

        builder.Services.AddInfrastructure(builder.Configuration);

        builder.Services.UseHiTechPaySdk(new()
        {
            PaymentServerAddress = builder.Configuration["PaymentServer:Url"],
            KeyStorageDirectory = builder.Configuration["PaymentServer:KeyStoragePath"]
        });

        return builder;
    }
}