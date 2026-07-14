using HiTechStore.Infrastructure.Data;
using HiTechStore.IntegrationTests.TestData;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;



namespace HiTechStore.IntegrationTests.Infrastructure;

public class HiTechStoreWebApplicationFactory : WebApplicationFactory<Program>
{
    private string _connectionString;
    public HiTechStoreWebApplicationFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.Remove(
                services.SingleOrDefault(d => d.ServiceType == typeof(IDbContextOptionsConfiguration<HiTechStoreDbContext>))!
            );

            services.AddDbContext<HiTechStoreDbContext>(options =>
            {
                options.UseNpgsql(_connectionString);
            });

            using var scope = services.BuildServiceProvider().CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<HiTechStoreDbContext>();

            db.Database.EnsureCreated();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "TestAuth";
                options.DefaultChallengeScheme = "TestAuth";
            })
            .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("TestAuth", options => { });
        });

        builder.ConfigureAppConfiguration(
               (context, config) =>
               {
                   config.AddInMemoryCollection(new Dictionary<string, string?>
                   {
                       ["AdminEmail"] = TestUsers.Admin.Email,
                       ["AdminPassword"] = TestUsers.Admin.Password
                   });
               }
           );
    }

    public async Task<TResult> ExecuteScopeAsync<TResult>(
        Func<IServiceProvider, Task<TResult>> action)
    {
        using var scope = Services.CreateScope();

        return await action(scope.ServiceProvider);
    }

    public async Task<TResult> UseServiceAsync<TService, TResult>(
       Func<TService, Task<TResult>> action)
    where TService : notnull
    {
        using var scope = Services.CreateScope();

        var requestedService = scope.ServiceProvider.GetRequiredService<TService>();
        return await action(requestedService);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
    }
}